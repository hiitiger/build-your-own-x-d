namespace MCompiler.CodeAnalysis.Syntax
{
    internal class WhileStatementSyntax : StatementSyntax
    {
        public SyntaxToken Keyword;
        public ExpressionSyntax Condition;
        public StatementSyntax Body;

        public WhileStatementSyntax(SyntaxToken keyword, ExpressionSyntax condition, StatementSyntax body)
        {
            this.Keyword = keyword;
            this.Condition = condition;
            this.Body = body;
        }

        public override SyntaxKind Kind => SyntaxKind.WhileStatement;
    }
}