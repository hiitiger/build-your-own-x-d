namespace MCompiler.CodeAnalysis.Binding
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(LabelSymbol symbol)
        {
            Symbol = symbol;
        }

        public override BoundNodeKind Kind => BoundNodeKind.LabelStatement;

        public LabelSymbol Symbol { get; }
    }
}