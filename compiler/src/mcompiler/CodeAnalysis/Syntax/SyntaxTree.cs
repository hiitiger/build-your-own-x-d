namespace MCompiler.CodeAnalysis.Syntax
{
    using System.Collections.Generic;
    using System.Linq;

    public sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken eof)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            Eof = eof;
        }

        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken Eof { get; }

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }
    }

}