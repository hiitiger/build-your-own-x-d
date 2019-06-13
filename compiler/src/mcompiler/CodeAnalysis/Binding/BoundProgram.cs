using System.Collections.Immutable;
using MCompiler.CodeAnalysis.Symbols;

namespace MCompiler.CodeAnalysis.Binding
{
    internal class BoundProgram
    {

        public BoundProgram(BoundBlockStatement statement, DiagnosticBag diagnostics, ImmutableDictionary<FunctionSymbol, BoundBlockStatement> functionBodies)
        {
            Statement = statement;
            Diagnostics = diagnostics;
            FunctionBodies = functionBodies;
        }

        public BoundBlockStatement Statement { get; }
        public DiagnosticBag Diagnostics;
        public ImmutableDictionary<FunctionSymbol, BoundBlockStatement> FunctionBodies;

    }
}