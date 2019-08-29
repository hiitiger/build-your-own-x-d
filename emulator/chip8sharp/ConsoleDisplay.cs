using System;

namespace chip8sharp
{
    public class ConsoleDisplay
    {
        public void Draw(byte[,] vram)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Green;
            for (int y = 0; y < CHIP8Spec.CHIP8_HEIGHT; ++y)
            {
                var line = "";
                for (int x = 0; x < CHIP8Spec.CHIP8_WIDTH; ++x)
                {
                    if (vram[y, x] != 0)
                        line += "*";
                    else
                        line += " ";
                }
                Console.WriteLine(line);
            }
            Console.ResetColor();
        }
    }
}
