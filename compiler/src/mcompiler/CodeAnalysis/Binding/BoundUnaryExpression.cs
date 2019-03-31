using System;

namespace MCompiler.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand)
        {
            Op = op;
            Operand = operand;
        }

        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
        public override Type Type => Operand.Type;

        public BoundUnaryOperator Op { get; }
        public BoundUnaryOperatorKind OperatorKind => Op.Kind;
        public BoundExpression Operand { get; }
    }
}