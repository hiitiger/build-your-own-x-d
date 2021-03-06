using System;
using System.Collections.Generic;
using System.Linq;
using MCompiler.CodeAnalysis;
using MCompiler.CodeAnalysis.Symbols;
using MCompiler.CodeAnalysis.Syntax;

namespace MCompiler
{
    internal sealed class MCRepl : Repl
    {
        private Compilation _previous;
        private bool _showTree;
        private bool _showProgram;
        private Dictionary<VariableSymbol, object> _variables = new Dictionary<VariableSymbol, object>();
        protected override bool IsCompleteSubmission(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            var continuousBlankLines = text.Split(Environment.NewLine).Reverse()
                                        .Where(s => string.IsNullOrEmpty(s))
                                        .Take(2).Count() == 2;
            if (continuousBlankLines)
                return true;

            var syntaxTree = SyntaxTree.Parse(text);
            if (syntaxTree.Root.Members.Last().GetLastToken().IsMissing)
                return false;

            return true;
        }


        protected override void RenderLine(string text)
        {
            var tokens = SyntaxTree.ParseTokens(text);
            foreach (var token in tokens)
            {
                var isKeyword = token.Kind.ToString().EndsWith("Keyword");
                var isOperator = SyntaxFacts.GetBinaryOperatorPrecedence(token.Kind) > 0 || SyntaxFacts.GetUnaryOperatorPrecedence(token.Kind) > 0;
                var isIdentifier = token.Kind == SyntaxKind.IdentifierToken;
                var isNumber = token.Kind == SyntaxKind.NumberToken;
                var isString = token.Kind == SyntaxKind.StringToken;

                if (isKeyword)
                    Console.ForegroundColor = ConsoleColor.Blue;
                else if (isIdentifier)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                else if (isOperator)
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                else if (isNumber)
                    Console.ForegroundColor = ConsoleColor.Cyan;
                else if (isString)
                    Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(token.Text);

                Console.ResetColor();
            }
        }

        protected override void EvaluateMetaCommand(string input)
        {
            switch (input)
            {
                case "#showTree":
                    _showTree = !_showTree;
                    Console.WriteLine($"show parse tree: {_showTree}");
                    break;
                case "#showProgram":
                    _showProgram = !_showProgram;
                    Console.WriteLine($"show bound tree: {_showProgram}");
                    break;
                case "#cls":
                    Console.Clear();
                    break;
                case "#reset":
                    _previous = null;
                    _variables.Clear();
                    break;
                default:
                    base.EvaluateMetaCommand(input);
                    break;
            }
        }

        protected override void EvaluateSubmission(string source)
        {
            var syntaxTree = SyntaxTree.Parse(source);

            var complilation = _previous == null ? new Compilation(syntaxTree) : _previous.ContinueWith(syntaxTree);

            var result = complilation.Evaluate(_variables);
            var diagnostics = result.Diagnostics.ToArray();

            if (_showTree)
                syntaxTree.Root.WriteTo(Console.Out);

            if (_showProgram)
                complilation.EmitTree(Console.Out);

            if (!diagnostics.Any())
            {
                if (result.Value != null)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(result.Value);
                    Console.ResetColor();
                }

                //only when complile with no errors
                _previous = complilation;
            }
            else
            {
                var text = syntaxTree.Text;
                foreach (var diagnostic in diagnostics.OrderBy(d => d.Span, new TextSpanComparer()))
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
        }
    }
}