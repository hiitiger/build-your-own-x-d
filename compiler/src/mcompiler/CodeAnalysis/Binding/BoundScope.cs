using System.Collections.Generic;
using System.Collections.Immutable;
using MCompiler.CodeAnalysis.Symbol;

namespace MCompiler.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        private Dictionary<string, VariableSymbol> _variables = new Dictionary<string, VariableSymbol>();
        public BoundScope Parent { get; }

        public BoundScope(BoundScope parent)
        {
            Parent = parent;
        }
        public bool TryDeclare(VariableSymbol variable)
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

        public bool TryLookup(string name, out VariableSymbol variable)
        {
            if (_variables.TryGetValue(name, out variable))
            {
                return true;
            }
            else
            {
                if (Parent == null)
                    return false;

                return Parent.TryLookup(name, out variable);
            }
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            return _variables.Values.ToImmutableArray();
        }
    }
}