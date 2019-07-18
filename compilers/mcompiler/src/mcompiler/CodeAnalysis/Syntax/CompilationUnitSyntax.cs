using System.Collections.Immutable;

namespace MCompiler.CodeAnalysis.Syntax
{
    public sealed class CompilationUnitSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

        public ImmutableArray<MemberSyntax> Members { get; }
        public SyntaxToken EofToken { get; }

        public CompilationUnitSyntax(ImmutableArray<MemberSyntax> members, SyntaxToken eofToken)
        {
            Members = members;
            EofToken = eofToken;
        }
    }
}