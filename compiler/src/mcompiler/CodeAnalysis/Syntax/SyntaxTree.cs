namespace MCompiler.CodeAnalysis.Syntax
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using MCompiler.CodeAnalysis.Text;

    public sealed class SyntaxTree
    {
        public SyntaxTree(SourceText text, ImmutableArray<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken eof)
        {
            Text = text;
            Diagnostics = diagnostics;
            Root = root;
            Eof = eof;
        }

        public SourceText Text { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken Eof { get; }


        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);
            return Parse(sourceText);
        }

        public static SyntaxTree Parse(SourceText text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            return ParseTokens(SourceText.From(text));
        }

        public static IEnumerable<SyntaxToken> ParseTokens(SourceText text)
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