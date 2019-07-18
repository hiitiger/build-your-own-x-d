using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MCompiler.CodeAnalysis.Symbols;

namespace MCompiler.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        // private Dictionary<string, VariableSymbol> _variables = new Dictionary<string, VariableSymbol>();
        // private Dictionary<string, FunctionSymbol> _functions = new Dictionary<string, FunctionSymbol>();
        private Dictionary<string, Symbol> _symbols = new Dictionary<string, Symbol>();
        public BoundScope Parent { get; }

        public BoundScope(BoundScope parent)
        {
            Parent = parent;
        }
        public bool TryDeclareVariable(VariableSymbol variable) => TryDeclareSymbol(variable);

        public bool TryLookupVariable(string name, out VariableSymbol variable) => TryLookupSymbol(name, out variable);

        private bool TryDeclareSymbol<TSymbol>(TSymbol symbol)
         where TSymbol : Symbol
        {
            if (_symbols.ContainsKey(symbol.Name))
                return false;

            _symbols.Add(symbol.Name, symbol);
            return true;
        }

        private bool TryLookupSymbol<TSymbol>(string name, out TSymbol symbol)
         where TSymbol : Symbol
        {
            symbol = null;
            if (_symbols.TryGetValue(name, out var declaredSymbol))
            {
                if (declaredSymbol is TSymbol matchingSymbol)
                {
                    symbol = matchingSymbol;
                    return true;
                }

                return false;
            }

            if (Parent == null)
                return false;

            return Parent.TryLookupSymbol(name, out symbol);
        }

        public bool TryDeclareFunction(FunctionSymbol function) => TryDeclareSymbol(function);

        public bool TryLookupFunction(string name, out FunctionSymbol function) => TryLookupSymbol(name, out function);

        public ImmutableArray<VariableSymbol> GetDeclaredVariables() => GetDeclaredSymbols<VariableSymbol>();

        public ImmutableArray<FunctionSymbol> GetDeclaredFunctions() => GetDeclaredSymbols<FunctionSymbol>();

        private ImmutableArray<TSymbol> GetDeclaredSymbols<TSymbol>()
            where TSymbol : Symbol
        {
            return _symbols.Values.OfType<TSymbol>().ToImmutableArray();
        }
    }
}