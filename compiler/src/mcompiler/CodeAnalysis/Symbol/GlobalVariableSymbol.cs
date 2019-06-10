namespace MCompiler.CodeAnalysis.Symbols
{
    public class GlobalVariableSymbol : VariableSymbol
    {
        internal GlobalVariableSymbol(string name, bool isReadonly, TypeSymbol type)
            : base(name, isReadonly, type)
        {
        }
        public override SymbolKind Kind => SymbolKind.GlobalVariable;
    }
}