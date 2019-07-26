namespace MCompiler.CodeAnalysis.Syntax
{
    public class NameExpressionSyntax : ExpressionSyntax
    {
        public NameExpressionSyntax(SyntaxToken identifierToken)
        {
            IdentifierToken = identifierToken;
        }

        public SyntaxToken IdentifierToken { get; }
        public override SyntaxKind Kind => SyntaxKind.NameExpression;

    }

}