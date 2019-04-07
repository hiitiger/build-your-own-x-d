namespace MCompiler.CodeAnalysis.Binding
{
    internal sealed class BoundVariableDeclarationStatement : BoundStatement
    {
        public BoundVariableDeclarationStatement(VariableSymbol variable, BoundExpression initializer)
        {
            Variable = variable;
            Initializer = initializer;
        }
        public override BoundNodeKind Kind => BoundNodeKind.VariableDeclarationStatement;

        public VariableSymbol Variable { get; }
        public BoundExpression Initializer { get; }
    }
}