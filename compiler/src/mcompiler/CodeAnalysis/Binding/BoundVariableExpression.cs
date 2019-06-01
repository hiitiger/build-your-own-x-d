using System;
using MCompiler.CodeAnalysis.Symbol;

namespace MCompiler.CodeAnalysis.Binding
{
    internal class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(VariableSymbol variable)
        {
            Variable = variable;
        }
        public override TypeSymbol Type => Variable.Type;
        public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
        public VariableSymbol Variable { get; }
    }
}