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

        //
        TrueKeyword,
        FalseKeyword,
        IndentifierToken,

        //
        LiteralExpression,
        BinaryExpression,
        UnaryExpression,
        ParenthesizedExpression,
        
    }

}