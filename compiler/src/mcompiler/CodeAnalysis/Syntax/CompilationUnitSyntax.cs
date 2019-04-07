namespace MCompiler.CodeAnalysis.Syntax
{
    public sealed class CompilationUnitSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

        public StatementSyntax Statement { get; }
        public SyntaxToken EofToken { get; }

        public CompilationUnitSyntax(StatementSyntax statement, SyntaxToken eofToken)
        {
            Statement = statement;
            EofToken = eofToken;
        }
    }
}