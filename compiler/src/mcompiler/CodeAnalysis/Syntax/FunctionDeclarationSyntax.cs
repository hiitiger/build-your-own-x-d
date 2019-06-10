namespace MCompiler.CodeAnalysis.Syntax
{
    public sealed class FunctionDeclarationSyntax : MemberSyntax
    {
        public FunctionDeclarationSyntax(SyntaxToken functionKeyword, SyntaxToken identifier, SyntaxToken openParenthesisToken, SeparatedSyntaxList<ParameterSyntax> paramters, SyntaxToken closeParenthesisToken, TypeClauseSyntax type, BlockStatementSyntax body)
        {

            FunctionKeyword = functionKeyword;
            Identifier = identifier;
            OpenParenthesisToken = openParenthesisToken;
            Paramters = paramters;
            CloseParenthesisToken = closeParenthesisToken;
            Type = type;
            Body = body;
        }

        public override SyntaxKind Kind => SyntaxKind.FunctionDeclaration;

        public SyntaxToken FunctionKeyword { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken OpenParenthesisToken { get; }
        public SeparatedSyntaxList<ParameterSyntax> Paramters { get; }
        public SyntaxToken CloseParenthesisToken { get; }
        public TypeClauseSyntax Type { get; }
        public BlockStatementSyntax Body { get; }
    }
}