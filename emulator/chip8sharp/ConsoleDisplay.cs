using System;
using System.Text;
using System.Threading;

namespace chip8sharp
{
    public class ConsoleDisplay
    {
        public ConsoleDisplay()
        {
            Console.Clear();
        }

        public void Draw(byte[,] vram)
        {
            var sb = new StringBuilder();
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Cyan;
            for (int y = 0; y < CHIP8Spec.CHIP8_HEIGHT; ++y)
            {
                var line = "";
                for (int x = 0; x < CHIP8Spec.CHIP8_WIDTH; ++x)
                {
                    if (vram[y, x] != 0)
                        line += "■";
                    else
                        line += " ";
                }
                sb.AppendLine(line);
            }
            Console.Write(sb);
            Console.ResetColor();
        }
    }
}
