using System;

namespace MCompiler.CodeAnalysis.Syntax
{
    internal static class SyntaxFacts
    {

        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.Plus:
                case SyntaxKind.Minus:
                    return 5;
                case SyntaxKind.Bang:
                    return 5;
                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.Star:
                case SyntaxKind.Slash:
                    return 4;
                case SyntaxKind.Plus:
                case SyntaxKind.Minus:
                    return 3;
                case SyntaxKind.AmpersandAmpersand:
                    return 2;
                case SyntaxKind.PipePipe:
                    return 1;
                default:
                    return 0;
            }
        }

        internal static SyntaxKind GetKeywordKind(string text)
        {
            switch (text)
            {
                case "true":
                    return SyntaxKind.TrueKeyword;
                case "false":
                    return SyntaxKind.FalseKeyword;
                default:
                    return SyntaxKind.IndentifierToken;
            }
        }
    }

}