using System;
using MCompiler.CodeAnalysis.Symbol;

namespace MCompiler.CodeAnalysis.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol Type { get; }
    }
}