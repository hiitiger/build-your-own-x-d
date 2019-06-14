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
        ConversionExpression,
        DoWhileStatement,
        RetrunStatement,
    }
}