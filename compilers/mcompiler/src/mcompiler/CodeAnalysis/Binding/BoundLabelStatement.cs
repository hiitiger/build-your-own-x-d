namespace MCompiler.CodeAnalysis.Binding
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(BoundLabel symbol)
        {
            Symbol = symbol;
        }

        public override BoundNodeKind Kind => BoundNodeKind.LabelStatement;

        public BoundLabel Symbol { get; }
    }
}