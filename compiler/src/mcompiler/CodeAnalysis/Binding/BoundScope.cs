using System.Collections.Generic;
using System.Collections.Immutable;
using MCompiler.CodeAnalysis.Symbol;

namespace MCompiler.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        private Dictionary<string, VariableSymbol> _variables = new Dictionary<string, VariableSymbol>();
        private Dictionary<string, FunctionSymbol> _functions = new Dictionary<string, FunctionSymbol>();
        public BoundScope Parent { get; }

        public BoundScope(BoundScope parent)
        {
            Parent = parent;
        }
        public bool TryDeclareVariable(VariableSymbol variable)
        {
            if (!_variables.ContainsKey(variable.Name))
            {
                _variables.Add(variable.Name, variable);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryLookupVariable(string name, out VariableSymbol variable)
        {
            if (_variables.TryGetValue(name, out variable))
            {
                return true;
            }
            else
            {
                if (Parent == null)
                    return false;

                return Parent.TryLookupVariable(name, out variable);
            }
        }

         public bool TryDeclareFunction(FunctionSymbol function)
        {
            if (!_functions.ContainsKey(function.Name))
            {
                _functions.Add(function.Name, function);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryLookupFunction(string name, out FunctionSymbol function)
        {
            if (_functions.TryGetValue(name, out function))
            {
                return true;
            }
            else
            {
                if (Parent == null)
                    return false;

                return Parent.TryLookupFunction(name, out function);
            }
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            return _variables.Values.ToImmutableArray();
        }

        public ImmutableArray<FunctionSymbol> GetDeclaredFunctions()
        {
            return _functions.Values.ToImmutableArray();
        }
    }
}