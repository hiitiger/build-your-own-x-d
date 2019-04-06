namespace MCompiler.CodeAnalysis.Syntax
{
    public sealed class CompilationUnitSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

        public ExpressionSyntax Exprssion { get; }
        public SyntaxToken EofToken { get; }

        public CompilationUnitSyntax(ExpressionSyntax expression, SyntaxToken eofToken)
        {
            Exprssion = expression;
            EofToken = eofToken;
        }
    }
}