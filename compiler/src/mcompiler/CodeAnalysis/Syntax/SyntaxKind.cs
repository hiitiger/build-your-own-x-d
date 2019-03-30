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

        EqualsEquals,
        BangEquals,

        //
        TrueKeyword,
        FalseKeyword,
        IndentifierToken,

        //
        LiteralExpression,
        BinaryExpression,
        UnaryExpression,
        ParenthesizedExpression,

        //
        Bang,
        AmpersandAmpersand,
        PipePipe,
    }

}