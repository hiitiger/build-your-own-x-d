using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace chip8sharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());

            var cpu = new CPU();
            using (var reader = new BinaryReader(new FileStream("roms/PONG2.ch8", FileMode.Open)))
            {
                List<byte> program = new List<byte>();

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    program.Add(reader.ReadByte());
                }

                cpu.Load(program.ToArray());
            }

            while (true)
            {
                try
                {
                    var s = cpu.Step();
                    if (s.vramUpdated)
                        Draw(cpu);
                    Thread.Sleep(2);
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static void Draw(CPU cpu)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < CHIP8Spec.CHIP8_HEIGHT; ++y)
            {
                var line = "";
                for (int x = 0; x < CHIP8Spec.CHIP8_WIDTH; ++x)
                {
                    if (cpu.VRam[y, x] != 0)
                        line += "*";
                    else
                        line += " ";
                }
                Console.WriteLine(line);
            }
        }
    }
}
