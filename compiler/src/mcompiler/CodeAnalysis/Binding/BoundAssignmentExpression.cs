using System;
using MCompiler.CodeAnalysis.Symbol;

namespace MCompiler.CodeAnalysis.Binding
{
    internal class BoundAssignmentExpression : BoundExpression
    {

        public BoundAssignmentExpression(VariableSymbol variable, BoundExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }
        public BoundExpression Expression;
        public override TypeSymbol Type => Expression.Type;
        public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;

        public VariableSymbol Variable { get; }
    }
}