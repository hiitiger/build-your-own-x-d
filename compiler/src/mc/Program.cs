using System;
using System.Collections.Generic;
using System.Linq;
using MCompiler.CodeAnalysis;
using MCompiler.CodeAnalysis.Syntax;

namespace MCompiler
{
    internal static class Program
    {
        private static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            /*
            ├───
            │
            └───
            ─
            */

            var marker = isLast ? "└───" : "├───";
            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);
            if (node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += isLast ? "    " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
            {
                PrettyPrint(child, indent, child == lastChild);
            }
        }

        static void Main(string[] args)
        {
            bool showTree = false;
            var variables = new Dictionary<VariableSymbol, object>();

            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }

                if (line == "#showTree")
                {
                    showTree = !showTree;
                    Console.WriteLine($"showTree: {showTree}");
                    continue;
                }
                else if (line == "#cls")
                {
                    Console.Clear();
                    continue;
                }

                var syntaxTree = SyntaxTree.Parse(line);
                var complilation = new Compilation(syntaxTree);
                var result = complilation.Evaluate(variables);
                var diagnostics = result.Diagnostics.ToArray();

                if (showTree)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    PrettyPrint(syntaxTree.Root);
                    Console.ResetColor();
                }

                if (diagnostics.Any())
                {
                    foreach (var x in diagnostics)
                    {
                        var prefix = line.Substring(0, x.Span.Start);
                        var error = line.Substring(x.Span.Start, x.Span.Length);
                        var suffix = line.Substring(x.Span.End);

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(x);
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
                else
                {
                    Console.WriteLine(result.Value);
                }
            }
        }
    }
}