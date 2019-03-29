namespace MCompiler.CodeAnalysis
{
    internal static class SyntaxFacts
    {

        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.Plus:
                case SyntaxKind.Minus:
                    return 3;
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
                    return 2;
                case SyntaxKind.Plus:
                case SyntaxKind.Minus:
                    return 1;
                default:
                    return 0;
            }
        }
    }

}