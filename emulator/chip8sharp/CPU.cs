using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace chip8sharp
{
    public struct OutputState
    {
        public bool vramUpdated;
        public bool beep;
    }

    public class CPU
    {
        public byte[] RAM = new byte[CHIP8Spec.CHIP8_RAM];
        public byte[] V = new byte[CHIP8Spec.CHIP8_REGISTER];
        public ushort PC = 0;
        public ushort I = 0;// address register
        public Stack<ushort> Stack = new Stack<ushort>();
        public byte DelayTimer = 0;
        public byte SoundTimer = 0;
        public bool[] Keyboard = new bool[16];
        public bool AwaitKey = false;
        public byte[,] VRam = new byte[CHIP8Spec.CHIP8_HEIGHT, CHIP8Spec.CHIP8_WIDTH];
        Random rnd = new Random();

        public void Load(byte[] data)
        {
            Array.Clear(RAM, 0, RAM.Length);

            Array.Copy(CHIP8Spec.FONT_SET, RAM, CHIP8Spec.FONT_SET.Length);
            for (int i = 0; i < data.Length; ++i)
            {
                var addr = 0x200 + i;
                if (addr < 4096)
                    RAM[addr] = data[i];
            }

            PC = 0x200;
        }

        public OutputState Step()
        {
            var opcode = (ushort)((RAM[PC] << 8) | RAM[PC + 1]);
            PC += 2;
            return ExecuteOpcode(opcode);
        }
        public OutputState ExecuteOpcode(ushort opcode)
        {
            var vramUpdated = false;
            if (AwaitKey)
            {
                throw new Exception("wait for key");
            }

            var nibbles = (nibble: (opcode & 0xF000) >> 12,
                           x: (opcode & 0x0F00) >> 8,
                           y: (opcode & 0x00F0) >> 4,
                           n: (opcode & 0x000F));

            var nnn = (ushort)(opcode & 0x0FFF);
            var nn = (byte)(opcode & 0x00FF);
            var nibble = nibbles.nibble;
            var x = nibbles.x;
            var y = nibbles.y;
            var n = nibbles.n;

            switch (nibbles)
            {
                case var _ when nibbles == (0x00, 0x00, 0x0e, 0x00):
                    ClearScreen();
                    break;
                case var _ when nibbles == (0x00, 0x00, 0x0e, 0x0e):
                    Ret();
                    break;
                case var _ when nibble == 0x01:
                    Jump(nnn);
                    break;
                case var _ when nibble == 0x02:
                    CallSunRoutine(nnn);
                    break;
                case var _ when nibble == 0x03:
                    SkipIf(V[x] == nn);
                    break;
                case var _ when nibble == 0x04:
                    SkipIf(V[x] != nn);
                    break;
                case var _ when (nibble, n) == (0x05, 0x00):
                    SkipIf(V[x] == V[y]);
                    break;
                case var _ when nibble == 0x06:
                    V[x] = nn;
                    break;
                case var _ when nibble == 0x07:
                    V[x] += nn;
                    break;
                case var _ when nibble == 0x08:
                    switch (n)
                    {
                        case 0x00:
                            V[x] = V[y];
                            break;
                        case 0x01:
                            V[x] |= V[y];
                            break;
                        case 0x02:
                            V[x] &= V[y];
                            break;
                        case 0x03:
                            V[x] ^= V[y];
                            break;
                        case 0x04:
                            var result = V[x] + V[y];
                            V[0x0f] = (byte)(result > 0xff ? 1 : 0);
                            V[x] = (byte)result;
                            break;
                        case 0x05:
                            V[0x0f] = (byte)(V[x] > V[y] ? 1 : 0);
                            V[x] = (byte)(V[x] - V[y]);
                            break;
                        case 0x06:
                            V[0x0f] = (byte)(V[x] & 0x01);
                            V[x] = (byte)(V[x] >> 1);
                            break;
                        case 0x07:
                            V[0x0f] = (byte)(V[y] > V[x] ? 1 : 0);
                            V[x] = (byte)(V[y] - V[x]);
                            break;
                        case 0x0e:
                            V[0x0f] = (byte)((byte)(V[x] & 0b10000000) >> 7);
                            V[x] = (byte)(V[x] << 1);
                            break;
                        default:
                            throw new Exception($"Unexpect opcode:{opcode.ToString("X4")}");
                    }
                    break;
                case var _ when (nibble, n) == (0x09, 0x00):
                    SkipIf(V[x] != V[y]);
                    break;
                case var _ when nibble == 0x0a:
                    I = nnn;
                    break;
                case var _ when nibble == 0x0b:
                    PC = (ushort)(V[0] + nnn);
                    break;
                case var _ when nibble == 0x0c:
                    V[x] = (byte)(rnd.Next(256) & nnn);
                    break;
                case var _ when nibble == 0x0d:
                    DrawSprite(V[x], V[y], (byte)n);
                    vramUpdated = true;
                    break;
                case var _ when (nibble, y, n) == (0x0e, 0x09, 0x0e):
                    SkipIf(Keyboard[V[x]]);
                    break;
                case var _ when (nibble, y, n) == (0x0e, 0x0a, 0x01):
                    SkipIf(!Keyboard[V[x]]);
                    break;
                case var _ when nibble == 0x0f:
                    var yn = (y, n);
                    switch (yn)
                    {
                        case var _ when (y, n) == (0x00, 0x07):
                            V[x] = DelayTimer;
                            break;
                        case var _ when (y, n) == (0x00, 0x0a):
                            AwaitKey = true;
                            PC -= 2;
                            break;
                        case var _ when (y, n) == (0x01, 0x05):
                            DelayTimer = V[x];
                            break;
                        case var _ when (y, n) == (0x01, 0x08):
                            SoundTimer = V[x];
                            break;
                        case var _ when (y, n) == (0x01, 0x0e):
                            I += V[x];
                            break;
                        case var _ when (y, n) == (0x02, 0x09):
                            I = (ushort)(V[x] * 5);
                            break;
                        case var _ when (y, n) == (0x03, 0x03):
                            RAM[I] = (byte)(V[x] / 100);
                            RAM[I + 1] = (byte)((V[x] % 100) / 10);
                            RAM[I + 2] = (byte)(V[x] % 10);
                            break;
                        case var _ when (y, n) == (0x05, 0x05):
                            for (int i = 0; i <= x; ++i)
                                RAM[I + i] = V[i];
                            break;
                        case var _ when (y, n) == (0x06, 0x05):
                            for (int i = 0; i <= x; ++i)
                                V[i] = RAM[I + i];
                            break;
                        default:
                            throw new Exception($"Unexpect opcode:{opcode.ToString("X4")}");
                    }
                    break;
                default:
                    throw new Exception($"Unexpect opcode:{opcode.ToString("X4")}");
            }

            var outputState = new OutputState();
            outputState.vramUpdated = vramUpdated;
            outputState.beep = SoundTimer > 0;
            return outputState;
        }


        private void ClearScreen() => Array.Clear(VRam, 0, VRam.Length);
        private void Ret() => PC = Stack.Pop();
        private void Jump(ushort nnn) => PC = nnn;

        private void CallSunRoutine(ushort nnn)
        {
            Stack.Push(PC);
            PC = nnn;
        }
        private void SkipIf(bool cond) => PC = (ushort)(PC + (cond ? 2 : 0));

        private void DrawSprite(byte x, byte y, byte n)
        {
            V[0xf] = 0;
            for (int height = 0; height < n; ++height)
            {
                var yi = (byte)((y + height) % CHIP8Spec.CHIP8_HEIGHT);
                for (int bit = 0; bit < 8; ++bit)
                {
                    var xi = (byte)((x + bit) % CHIP8Spec.CHIP8_WIDTH);
                    var color = (RAM[I + height] >> (7 - bit)) & 1;
                    if (color == 1 && VRam[yi, xi] != 0)
                        V[0x0f] = 1;
                    VRam[yi, xi] ^= (byte)color;
                }
            }
        }
    }
}
