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
                    return 6;
                case SyntaxKind.Bang:
                    return 6;
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
                    return 5;
                case SyntaxKind.Plus:
                case SyntaxKind.Minus:
                    return 4;
                case SyntaxKind.EqualsEquals:
                case SyntaxKind.BangEquals:
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