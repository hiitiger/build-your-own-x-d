namespace MCompiler.CodeAnalysis.Syntax
{
    public sealed class VariableDeclarationStatementSyntax : StatementSyntax
    {
        public VariableDeclarationStatementSyntax(SyntaxToken keywordToken, SyntaxToken identifierToken, TypeClauseSyntax typeClause, SyntaxToken equalsToken, ExpressionSyntax initializer)
        {
            KeywordToken = keywordToken;
            IdentifierToken = identifierToken;
            TypeClause = typeClause;
            EqualsToken = equalsToken;
            Initializer = initializer;
        }

        public override SyntaxKind Kind => SyntaxKind.VariableDeclarationStatement;

        public SyntaxToken KeywordToken { get; }
        public SyntaxToken IdentifierToken { get; }
        public TypeClauseSyntax TypeClause { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Initializer { get; }
    }
}