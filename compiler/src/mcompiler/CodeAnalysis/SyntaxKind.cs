namespace MCompiler.CodeAnalysis
{
    enum SyntaxKind
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
        NumberExpression,
        BinaryExpression,
        ParenthesizedExpression
    }

}