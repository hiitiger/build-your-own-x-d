using System;

namespace chip8sharp
{
    class InputDriver
    {
        public bool[] keyboard = new bool[16];

        public void HandleKey()
        {
            Array.Clear(keyboard, 0, keyboard.Length);
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);

                var keyIndex = new Func<ConsoleKey, int>((ConsoleKey i) =>
                 {
                     switch (i)
                     {
                         case ConsoleKey.D1: return 1;
                         case ConsoleKey.D2: return 2;
                         case ConsoleKey.D3: return 3;
                         case ConsoleKey.D4: return 0xC;
                         case ConsoleKey.Q: return 0x4;
                         case ConsoleKey.W: return 0x5;
                         case ConsoleKey.E: return 0x6;
                         case ConsoleKey.R: return 0xD;
                         case ConsoleKey.A: return 0x7;
                         case ConsoleKey.S: return 0x8;
                         case ConsoleKey.D: return 0x9;
                         case ConsoleKey.F: return 0xE;
                         case ConsoleKey.Z: return 0xA;
                         case ConsoleKey.X: return 0x0;
                         case ConsoleKey.C: return 0xB;
                         case ConsoleKey.V: return 0xF;
                         default: return -1;
                     }
                 })(key.Key);

                if (keyIndex != -1)
                {
                    keyboard[keyIndex] = true;
                }
            }
        }
    }
}
