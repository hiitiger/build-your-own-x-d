namespace MCompiler.CodeAnalysis.Syntax
{
    internal sealed class BreakStatementSyntax : StatementSyntax
    {
        public SyntaxToken BreakKeyword;

        public BreakStatementSyntax(SyntaxToken breakKeyword)
        {
            BreakKeyword = breakKeyword;
        }

        public override SyntaxKind Kind => SyntaxKind.BreakStatement;
    }
}