using MCompiler.CodeAnalysis.Symbols;

namespace MCompiler.CodeAnalysis.Binding
{
    internal sealed class Conversion
    {
        public static readonly Conversion None = new Conversion(false, false, false);
        public static readonly Conversion Identity = new Conversion(true, true, true);
        public static readonly Conversion Implicit = new Conversion(true, false, true);
        public static readonly Conversion Explicit = new Conversion(true, false, false);

        public Conversion(bool exists, bool isIdentity, bool isImplicit)
        {
            Exists = exists;
            IsIdentity = isIdentity;
            IsImplicit = isImplicit;
        }

        public bool Exists { get; }
        public bool IsIdentity { get; }
        public bool IsImplicit { get; }
        public bool IsExplicit => Exists && !IsImplicit;

        public static Conversion Classify(TypeSymbol from, TypeSymbol to)
        {
            if (from == to)
                return Conversion.Identity;

            if (from == TypeSymbol.Int
               || from == TypeSymbol.Bool)
            {
                if (to == TypeSymbol.String)
                    return Conversion.Explicit;
            }

            if (from == TypeSymbol.String)
            {
                if (to == TypeSymbol.Int)
                    return Conversion.Explicit;
            }

            return Conversion.None;
        }
    }
}