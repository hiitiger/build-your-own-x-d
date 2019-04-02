using Xunit;
using MCompiler.CodeAnalysis.Syntax;
using System.Collections.Generic;
using System;
using MCompiler.CodeAnalysis;

namespace MCompiler.Tests.CodeAnalysis.Syntax
{
    public class EvaluationTests
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("-1", -1)]
        [InlineData("+1", 1)]
        [InlineData("1 + 1", 2)]
        [InlineData("4 - 2", 2)]
        [InlineData("2 * 2", 4)]
        [InlineData("4 / 2", 2)]
        [InlineData("(4 + 2)", 6)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!false", true)]
        [InlineData("!true", false)]

        [InlineData("1 == 1", true)]
        [InlineData("1 == 2", false)]
        [InlineData("1 != 2", true)]
        [InlineData("1 != 1", false)]

        [InlineData("true && true", true)]
        [InlineData("true && false", false)]
        [InlineData("false && false", false)]
        [InlineData("false || false", false)]
        [InlineData("true || false", true)]
        [InlineData("true || true", true)]

        public void EvaluationTests_GetText_RoundTrips(string text, object value)
        {
            var variables = new Dictionary<VariableSymbol, object>();

            var compilation = new Compilation(SyntaxTree.Parse(text));
            var evaluatedResult = compilation.Evaluate(variables);

            Assert.Empty(evaluatedResult.Diagnostics);
            Assert.Equal(evaluatedResult.Value, value);
        }
    }
}