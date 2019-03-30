namespace MCompiler.CodeAnalysis.Syntax
{
    using System.Collections.Generic;

    public class AssignmentExpressionSyntax : ExpressionSyntax
    {
        public AssignmentExpressionSyntax(SyntaxToken identifierToken, SyntaxToken equalsToken, ExpressionSyntax expression)
        {
            IdentifierToken = identifierToken;
            EqualsToken = equalsToken;
            Expression = expression;
        }

        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Expression { get; }

        public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return IdentifierToken;
            yield return EqualsToken;
            yield return Expression;
        }
    }

}