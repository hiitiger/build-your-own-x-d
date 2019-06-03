using System.Collections.Immutable;

namespace MCompiler.CodeAnalysis.Symbol
{
    public sealed class FunctionSymbol : Symbol
    {
        public FunctionSymbol(string name, ImmutableArray<ParameterSymbol> parameters, TypeSymbol type)
        : base(name)
        {
            Parameters = parameters;
            Type = type;
        }

        public override SymbolKind Kind => SymbolKind.Function;

        public ImmutableArray<ParameterSymbol> Parameters { get; }
        public TypeSymbol Type { get; }
    }
}