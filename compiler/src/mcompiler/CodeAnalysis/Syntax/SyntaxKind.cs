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
        OpenBraceToken,
        CloseBraceToken,
        BadToken,
        EOFToken,

        BangToken,

        AmpersandAmpersandToken,
        PipePipeToken,

        EqualsEqualsToken,
        BangEqualsToken,

        EqualsToken,

        LessToken,
        LessEqualsToken,
        GreaterToken,
        GreaterEqualsToken,
        //
        TrueKeyword,
        FalseKeyword,
        LetKeyword,
        VarKeyword,
        IfKeyword,
        ElseKeyword,
        WhileKeyword,

        IdentifierToken,

        //
        LiteralExpression,
        BinaryExpression,
        UnaryExpression,
        ParenthesizedExpression,

        //expression
        NameExpression,
        AssignmentExpression,

        //
        CompilationUnit,

        ElseClause,

        //statament
        BlockStatement,
        ExpressionStatement,
        VariableDeclarationStatement,
        IfStatement,
        WhileStatement,
    }

}