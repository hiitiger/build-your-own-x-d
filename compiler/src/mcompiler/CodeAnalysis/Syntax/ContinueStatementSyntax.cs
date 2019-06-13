namespace MCompiler.CodeAnalysis.Syntax
{
    internal sealed class ContinueStatementSyntax : StatementSyntax
    {
        public SyntaxToken ContinueKeyword;

        public ContinueStatementSyntax(SyntaxToken continueKeyword)
        {
            ContinueKeyword = continueKeyword;
        }

        public override SyntaxKind Kind => SyntaxKind.ContinueStatement;
    }
}