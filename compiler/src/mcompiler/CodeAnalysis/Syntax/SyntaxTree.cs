namespace MCompiler.CodeAnalysis.Syntax
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    public sealed class SyntaxTree
    {
        public SyntaxTree(ImmutableArray<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken eof)
        {
            Diagnostics = diagnostics;
            Root = root;
            Eof = eof;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken Eof { get; }

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            Lexer lexer = new Lexer(text);
            while (true)
            {
                var token = lexer.Lex();
                if (token.Kind == SyntaxKind.EOFToken)
                    break;
                yield return token;
            }
        }
    }

}