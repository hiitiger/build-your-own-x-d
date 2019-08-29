using System;
using System.Collections.Generic;
using System.IO;

namespace chip8sharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());

            var cpu = new CPU();
            using (var reader = new BinaryReader(new FileStream("roms/pong.ch8", FileMode.Open)))
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
                cpu.Step();
                // try
                // {
                //     cpu.Step();
                // }
                // catch (System.Exception e)
                // {
                //     Console.WriteLine(e.Message);
                // }
            }
        }
    }
}
