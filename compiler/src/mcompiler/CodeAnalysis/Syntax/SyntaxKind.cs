namespace MCompiler.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        Number,
        WhiteSpace,
        Plus,
        Minus,
        Star,
        Slash,
        OpenParenthesis,
        CloseParenthesis,
        BadToken,
        EOF,
        LiteralExpression,
        BinaryExpression,
        UnaryExpression,
        ParenthesizedExpression,
    }

}