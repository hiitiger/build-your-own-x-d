namespace MCompiler.CodeAnalysis.Syntax
{
    internal sealed class ForStatementSyntax : StatementSyntax
    {
        public ForStatementSyntax(SyntaxToken keyword, ExpressionSyntax initializer, ExpressionSyntax condition, ExpressionSyntax loop, StatementSyntax body)
        {
            Keyword = keyword;
            Initializer = initializer;
            Condition = condition;
            Loop = loop;
            Body = body;
        }
        public override SyntaxKind Kind => SyntaxKind.ForStatement;

        public SyntaxToken Keyword { get; }
        public ExpressionSyntax Initializer { get; }
        public ExpressionSyntax Condition { get; }
        public ExpressionSyntax Loop { get; }
        public StatementSyntax Body { get; }
    }
}