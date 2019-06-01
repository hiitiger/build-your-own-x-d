using System;
using MCompiler.CodeAnalysis.Symbol;

namespace MCompiler.CodeAnalysis.Binding
{

    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator op, BoundExpression right)
        {
            Left = left;
            Op = op;
            Right = right;
        }

        public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
        public override TypeSymbol Type => Op.Type;

        public BoundExpression Left { get; }
        public BoundBinaryOperator Op { get; }
        public BoundBinaryOperatorKind OperatorKind => Op.Kind;
        public BoundExpression Right { get; }
    }
}