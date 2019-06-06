using MCompiler.CodeAnalysis.Symbols;

namespace MCompiler.CodeAnalysis.Binding
{
    internal class BoundConversionExpression : BoundExpression
    {

        public BoundConversionExpression(TypeSymbol type, BoundExpression expression)
        {
            this.Type = type;
            this.Expression = expression;
        }

        public override TypeSymbol Type { get; }
        public override BoundNodeKind Kind => BoundNodeKind.ConversionExpression;
        public BoundExpression Expression;

    }
}