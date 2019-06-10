using System.Collections.Immutable;
using MCompiler.CodeAnalysis.Symbols;

namespace MCompiler.CodeAnalysis.Binding
{
    internal class BoundProgram
    {
        public BoundGlobalScope GlobalScope;
        public DiagnosticBag Diagnostics;
        public ImmutableDictionary<FunctionSymbol, BoundBlockStatement> FunctionBodies;

        public BoundProgram(BoundGlobalScope globalScope, DiagnosticBag diagnostics, ImmutableDictionary<FunctionSymbol, BoundBlockStatement> functionBodies)
        {
            GlobalScope = globalScope;
            Diagnostics = diagnostics;
            FunctionBodies = functionBodies;
        }
    }
}