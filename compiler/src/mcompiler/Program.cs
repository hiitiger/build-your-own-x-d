using System;
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
                var binder = new CodeAnalysis.Binding.Binder();
                var boundExpression = binder.BindExpression(syntaxTree.Root);
                var diagnostics = syntaxTree.Diagnostics.Concat(binder.Diagnostics).ToArray();

                if (showTree)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    PrettyPrint(syntaxTree.Root);
                    Console.ResetColor();
                }

                if (diagnostics.Any())
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    foreach (var x in diagnostics)
                    {
                        Console.WriteLine(x);
                    }
                    Console.ResetColor();
                }
                else
                {
                    var e = new Evaluator(boundExpression);
                    var res = e.Evaluate();
                    Console.WriteLine(res);
                }

            }
        }
    }

}