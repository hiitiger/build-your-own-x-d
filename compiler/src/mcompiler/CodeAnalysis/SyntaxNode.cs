namespace MCompiler.CodeAnalysis
{
    using System.Collections.Generic;

    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }

}