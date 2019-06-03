using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MCompiler.CodeAnalysis.Syntax
{
    public abstract class SeparatedSyntaxList
    {
        public abstract ImmutableArray<SyntaxNode> GetWithSeparators();
    }
    public sealed class SeparatedSyntaxList<T> : SeparatedSyntaxList, IEnumerable<T>
        where T : SyntaxNode
    {
        public SeparatedSyntaxList(ImmutableArray<SyntaxNode> nodesAndSeparators)
        {
            _nodesAndSeparators = nodesAndSeparators;
        }
        private readonly ImmutableArray<SyntaxNode> _nodesAndSeparators;
        public int Count => (_nodesAndSeparators.Length + 1) / 2;
        public T this[int index] => (T)_nodesAndSeparators[index * 2];

        public SyntaxToken GetSeparator(int index) => (SyntaxToken)_nodesAndSeparators[index * 2 + 1];

        public override ImmutableArray<SyntaxNode> GetWithSeparators() => _nodesAndSeparators;

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i != Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}