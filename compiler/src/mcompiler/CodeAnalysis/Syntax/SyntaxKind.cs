namespace MCompiler.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        NumberToken,
        StringToken,

        WhiteSpaceToken,

        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,

        CommaToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        OpenBraceToken,
        CloseBraceToken,
        BadToken,
        EOFToken,

        BangToken,

        AmpersandAmpersandToken,
        PipePipeToken,
        AmpersandToken,
        PipeToken,
        HatToken,
        TildeToken,
        EqualsEqualsToken,
        BangEqualsToken,

        EqualsToken,

        LessToken,
        LessEqualsToken,
        GreaterToken,
        GreaterEqualsToken,
        ColonToken,

        //
        TrueKeyword,
        FalseKeyword,
        LetKeyword,
        VarKeyword,
        IfKeyword,
        ElseKeyword,
        WhileKeyword,
        ForKeyword,
        DoKeyword,
        FunctionKeyword,
        BreakKeyword,
        ContinueKeyword,
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
        GlobalStatement,
        FunctionDeclaration,
        Parameter,
        ElseClause,
        TypeClause,

        //statament
        BlockStatement,
        ExpressionStatement,
        VariableDeclarationStatement,
        IfStatement,
        WhileStatement,
        ForStatement,
        CallExpression,
        DoWhileStatement,
        BreakStatement,
        ContinueStatement,
    }

}