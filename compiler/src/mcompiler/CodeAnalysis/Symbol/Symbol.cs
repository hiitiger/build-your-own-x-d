namespace MCompiler.CodeAnalysis.Symbols
{
    public abstract class Symbol
    {
        internal Symbol(string name)
        {
            Name = name;
        }
        public string Name { get; }

        public abstract SymbolKind Kind { get; }

        public override string ToString() => Name;
    }
}