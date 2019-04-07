namespace MCompiler.CodeAnalysis.Syntax
{
    public sealed class VariableDeclarationStatementSyntax : StatementSyntax
    {
        public VariableDeclarationStatementSyntax(SyntaxToken keywordToken, SyntaxToken identifierToken, SyntaxToken equalsToken, ExpressionSyntax initializer)
        {
            KeywordToken = keywordToken;
            IdentifierToken = identifierToken;
            EqualsToken = equalsToken;
            Initializer = initializer;
        }

        public override SyntaxKind Kind => SyntaxKind.VariableDeclarationStatement;

        public SyntaxToken KeywordToken { get; }
        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Initializer { get; }
    }
}