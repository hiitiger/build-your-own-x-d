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
            using (var reader = new BinaryReader(new FileStream("roms/TETRIS.ch8", FileMode.Open)))
            {
                List<byte> program = new List<byte>();

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    program.Add(reader.ReadByte());
                }

                cpu.Load(program.ToArray());
            }

            var display = new ConsoleDisplay();
            while (true)
            {
                try
                {
                    var s = cpu.Step();
                    if (s.vramUpdated)
                        display.Draw(cpu.VRam);
                    Thread.Sleep(10);
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
