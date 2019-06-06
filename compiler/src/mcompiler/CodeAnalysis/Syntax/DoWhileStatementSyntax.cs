namespace MCompiler.CodeAnalysis.Syntax
{
    internal sealed class DoWhileStatementSyntax : StatementSyntax
    {
        public DoWhileStatementSyntax(SyntaxToken doKeyword, BlockStatementSyntax doStatement, SyntaxToken whileKeyword, ExpressionSyntax condition)
        {
            DoKeyword = doKeyword;
            Statament = doStatement;
            WhileKeyword = whileKeyword;
            Condition = condition;
        }

        public override SyntaxKind Kind => SyntaxKind.DoWhileStatement;

        public SyntaxToken DoKeyword { get; }
        public BlockStatementSyntax Statament { get; }
        public SyntaxToken WhileKeyword { get; }
        public ExpressionSyntax Condition { get; }
    }
}