using System;

namespace MCompiler.CodeAnalysis.Symbol
{
    public sealed class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol Int = new TypeSymbol("int");
        public static readonly TypeSymbol Bool = new TypeSymbol("bool");
        public static readonly TypeSymbol String = new TypeSymbol("string");
        public TypeSymbol(string name)
        : base(name)
        {

        }

        public override SymbolKind Kind => SymbolKind.Type;
    }
}