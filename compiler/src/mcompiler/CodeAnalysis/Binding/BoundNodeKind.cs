namespace MCompiler.CodeAnalysis.Binding
{
    public enum BoundNodeKind
    {
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,
        CallExpression,
        ConversionExpression,

        ErrorExpression,

        //
        BlockStatement,
        ExpressionStatement,
        VariableDeclarationStatement,
        IfStatement,
        WhileStatement,
        ForStatement,
        GotoStatement,
        LabelStatement,
        ConditionalGotoStatement,
        DoWhileStatement,
        ReturnStatement,
    }
}