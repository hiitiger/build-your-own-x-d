using System.IO;

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

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }

        public void WriteTo(TextWriter writer)
        {
            SymbolPrinter.WriteTo(this, writer);
        }

    }
}