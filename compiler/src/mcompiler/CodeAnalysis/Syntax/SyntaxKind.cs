namespace MCompiler.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        NumberToken,
        WhiteSpaceToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EOFToken,


        BangToken,
        AmpersandAmpersandToken,
        PipePipeToken,

        EqualsEqualsToken,
        BangEqualsToken,

        EqualsToken,

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
        NameExpression,
        AssignmentExpression,
    }

}