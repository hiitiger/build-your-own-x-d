using System;
using Xunit;

using MCompiler.CodeAnalysis.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace MCompiler.Tests.CodeAnalysis.Syntax
{

    public class LexerTests
    {
        [Fact]
        public void Lexer_Lexes_AllTokens()
        {
            var tokenKinds = Enum.GetValues(typeof(SyntaxKind))
                              .Cast<SyntaxKind>()
                              .Where(k => k.ToString().EndsWith("Keyword") || k.ToString().EndsWith("Token"))
                              .ToList();
            var testedTokenKinds = GetTokens().Concat(GetSeperators()).Select(t => t.kind);
            var untestedTokenKinds = new SortedSet<SyntaxKind>(tokenKinds);
            untestedTokenKinds.ExceptWith(testedTokenKinds);
            untestedTokenKinds.Remove(SyntaxKind.BadToken);
            untestedTokenKinds.Remove(SyntaxKind.EOFToken);
            Assert.Empty(untestedTokenKinds);
        }

        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void Lexer_Lexes_Tokens(SyntaxKind kind, string text)
        {
            var tokens = SyntaxTree.ParseTokens(text);
            var token = Assert.Single(tokens);
            Assert.Equal(kind, token.Kind);
            Assert.Equal(text, token.Text);
        }


        [Theory]
        [MemberData(nameof(GetTokenPairsData))]
        public void Lexer_Lexes_TokenPairs(SyntaxKind kind1, string text1, SyntaxKind kind2, string text2)
        {
            var text = text1 + text2;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(2, tokens.Length);

            Assert.Equal(tokens[0].Kind, kind1);
            Assert.Equal(tokens[0].Text, text1);
            Assert.Equal(tokens[1].Kind, kind2);
            Assert.Equal(tokens[1].Text, text2);
        }


        [Theory]
        [MemberData(nameof(GetTokenPairsWithSepartorData))]
        public void Lexer_Lexes_TokenPairsWithSeparator(SyntaxKind kind1, string text1, SyntaxKind separatorKind, string separatorText, SyntaxKind kind2, string text2)
        {
            var text = text1 + separatorText + text2;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(3, tokens.Length);

            Assert.Equal(tokens[0].Kind, kind1);
            Assert.Equal(tokens[0].Text, text1);
            Assert.Equal(tokens[1].Kind, separatorKind);
            Assert.Equal(tokens[1].Text, separatorText);
            Assert.Equal(tokens[2].Kind, kind2);
            Assert.Equal(tokens[2].Text, text2);
        }


        public static IEnumerable<object[]> GetTokensData()
        {
            foreach (var t in GetTokens().Concat(GetSeperators()))
                yield return new object[] { t.kind, t.text };
        }

        public static IEnumerable<object[]> GetTokenPairsData()
        {
            foreach (var t in GetTokenPairs())
                yield return new object[] { t.kind1, t.text1, t.kind2, t.text2 };
        }

        public static IEnumerable<object[]> GetTokenPairsWithSepartorData()
        {
            foreach (var t in GetTokenPairsWithSeparator())
                yield return new object[] { t.kind1, t.text1, t.separatorKind, t.separatorText, t.kind2, t.text2 };
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetTokens()
        {
            var fixedTokes = Enum.GetValues(typeof(SyntaxKind))
                                .Cast<SyntaxKind>()
                                .Select(k => (kind: k, text: SyntaxFacts.GetText(k)))
                                .Where(v => v.text != null);

            var dynamicTokens = new[] {
                (SyntaxKind.NumberToken, "1"),
                (SyntaxKind.NumberToken, "12"),
                (SyntaxKind.IndentifierToken, "a"),
                (SyntaxKind.IndentifierToken, "abc"),
            };


            return dynamicTokens.Concat(fixedTokes);
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetSeperators()
        {
            return new[] {
                (SyntaxKind.WhiteSpaceToken, " "),
                (SyntaxKind.WhiteSpaceToken, "  "),
                (SyntaxKind.WhiteSpaceToken, "\r"),
                (SyntaxKind.WhiteSpaceToken, "\n"),
                (SyntaxKind.WhiteSpaceToken, "\r\n")
            };
        }

        private static bool RequiresSeparator(SyntaxKind kind1, SyntaxKind kind2)
        {
            var t1IsKeyword = kind1.ToString().EndsWith("Keyword");
            var t2IsKeyword = kind2.ToString().EndsWith("Keyword");

            if (kind1 == SyntaxKind.IndentifierToken && kind2 == SyntaxKind.IndentifierToken)
                return true;

            if (t1IsKeyword && t2IsKeyword)
                return true;

            if (t1IsKeyword && kind2 == SyntaxKind.IndentifierToken)
                return true;

            if (kind1 == SyntaxKind.IndentifierToken && t2IsKeyword)
                return true;

            if (kind1 == SyntaxKind.NumberToken && kind2 == SyntaxKind.NumberToken)
                return true;

            if (kind1 == SyntaxKind.BangToken && kind2 == SyntaxKind.EqualsToken)
                return true;

            if (kind1 == SyntaxKind.BangToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return true;

            if (kind1 == SyntaxKind.EqualsToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return true;

            if (kind1 == SyntaxKind.EqualsToken && kind2 == SyntaxKind.EqualsToken)
                return true;


            return false;
        }

        private static IEnumerable<(SyntaxKind kind1, string text1, SyntaxKind kind2, string text2)> GetTokenPairs()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if (!RequiresSeparator(t1.kind, t2.kind))
                        yield return (t1.kind, t1.text, t2.kind, t2.text);
                }
            }
        }


        private static IEnumerable<(SyntaxKind kind1, string text1, SyntaxKind separatorKind, string separatorText, SyntaxKind kind2, string text2)> GetTokenPairsWithSeparator()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if (RequiresSeparator(t1.kind, t2.kind))
                    {
                        foreach (var s in GetSeperators())
                        {
                            yield return (t1.kind, t1.text, s.kind, s.text, t2.kind, t2.text);
                        }
                    }
                }
            }
        }
    }
}
