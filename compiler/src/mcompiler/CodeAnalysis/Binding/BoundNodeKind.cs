namespace MCompiler.CodeAnalysis.Binding
{
    public enum BoundNodeKind
    {
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,

        //
        BlockStatement,
        ExpressionStatement,
        VariableDeclarationStatement,
        IfStatement
    }
}