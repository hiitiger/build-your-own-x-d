namespace MCompiler.CodeAnalysis
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
        ParenthesizedExpression
    }

}