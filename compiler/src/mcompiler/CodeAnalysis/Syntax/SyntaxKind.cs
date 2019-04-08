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

        //statament
        BlockStatement,
        ExpressionStatement,
        VariableDeclarationStatement,
        
    }

}