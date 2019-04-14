using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCompiler.CodeAnalysis;
using MCompiler.CodeAnalysis.Syntax;

namespace MCompiler
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            bool showTree = false;
            bool showProgram = false;
            var variables = new Dictionary<VariableSymbol, object>();
            var textBuilder = new StringBuilder();
            Compilation previous = null;

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (textBuilder.Length == 0)
                    Console.Write("→ ");
                else
                    Console.Write("· ");
                Console.ResetColor();

                var input = Console.ReadLine();
                var isBlank = string.IsNullOrWhiteSpace(input);

                if (textBuilder.Length == 0)
                {
                    if (isBlank)
                    {
                        break;
                    }
                    else if (input == "#showTree")
                    {
                        showTree = !showTree;
                        Console.WriteLine($"show parse tree: {showTree}");
                        continue;
                    }
                    else if (input == "#showProgram")
                    {
                        showProgram = !showProgram;
                        Console.WriteLine($"show bound tree: {showProgram}");
                        continue;
                    }
                    else if (input == "#cls")
                    {
                        Console.Clear();
                        continue;
                    }
                    else if (input == "#reset")
                    {
                        previous = null;
                        variables.Clear();
                        continue;
                    }
                }

                textBuilder.AppendLine(input);
                var syntaxTree = SyntaxTree.Parse(textBuilder.ToString());

                if (!isBlank && syntaxTree.Diagnostics.Any())
                    continue;

                var complilation = previous == null ? new Compilation(syntaxTree) : previous.ContinueWith(syntaxTree);

                var result = complilation.Evaluate(variables);
                var diagnostics = result.Diagnostics.ToArray();
                var text = syntaxTree.Text;

                if (showTree)
                    syntaxTree.Root.WriteTo(Console.Out);

                if (showProgram)
                    complilation.EmitTree(Console.Out);

                if (!diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(result.Value);
                    Console.ResetColor();
                    //only when complile with no errors
                    previous = complilation;
                }
                else
                {
                    foreach (var diagnostic in diagnostics)
                    {
                        var lineIndex = text.GetLineIndex(diagnostic.Span.Start);
                        var lineNumber = lineIndex + 1;
                        var textLine = text.Lines[lineIndex];
                        var character = diagnostic.Span.Start - textLine.Start + 1;

                        var prefix = text.ToString(textLine.Start, diagnostic.Span.Start - textLine.Start);
                        var error = text.ToString(diagnostic.Span.Start, diagnostic.Span.Length);
                        var suffix = text.ToString(diagnostic.Span.End, textLine.End - diagnostic.Span.End);

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write($"({lineNumber}, {character}):");
                        Console.WriteLine(diagnostic);
                        Console.ResetColor();

                        Console.Write("    ");
                        Console.Write(prefix);
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(error);
                        Console.ResetColor();
                        Console.Write(suffix);

                        Console.WriteLine();
                    }
                }
                textBuilder.Clear();
            }
        }
    }
}