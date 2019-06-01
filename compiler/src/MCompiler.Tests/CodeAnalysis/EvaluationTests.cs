using Xunit;
using MCompiler.CodeAnalysis.Syntax;
using System.Collections.Generic;
using System;
using MCompiler.CodeAnalysis;
using MCompiler.CodeAnalysis.Symbol;

namespace MCompiler.Tests.CodeAnalysis
{
    public class EvaluationTests
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("-1", -1)]
        [InlineData("+1", 1)]
        [InlineData("~1", -2)]
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

        [InlineData("2 > 1", true)]
        [InlineData("2 > 2", false)]
        [InlineData("2 >= 2", true)]
        [InlineData("2 >= 3", false)]
        [InlineData("1 < 2", true)]
        [InlineData("1 < 1", false)]
        [InlineData("1 <= 1", true)]
        [InlineData("2 <= 1", false)]
        [InlineData("1 | 2", 3)]
        [InlineData("1 | 0", 1)]
        [InlineData("1 & 2", 0)]
        [InlineData("1 & 0", 0)]
        [InlineData("1 ^ 0", 1)]
        [InlineData("1 ^ 1", 0)]
        [InlineData("1 ^ 3", 2)]

        [InlineData("true && true", true)]
        [InlineData("true && false", false)]
        [InlineData("false && false", false)]
        [InlineData("false || false", false)]
        [InlineData("true || false", true)]
        [InlineData("true || true", true)]
        [InlineData("true | true", true)]
        [InlineData("true | false", true)]
        [InlineData("false | true", true)]
        [InlineData("false | false", false)]
        [InlineData("true & true", true)]
        [InlineData("true & false", false)]
        [InlineData("false & true", false)]
        [InlineData("false & false", false)]
        [InlineData("true ^ true", false)]
        [InlineData("true ^ false", true)]
        [InlineData("false ^ true", true)]
        [InlineData("false ^ false", false)]
        

        [InlineData("{var x = 10 (x = 5) * 5}", 25)]
        [InlineData("{var x = 0 if x == 0 x = 10 x}", 10)]
        [InlineData("{var x = 0 if x == 1 x = 10 else x = 20 x}", 20)]
        [InlineData("{var x = 0 while x < 10 x = x+1 x}", 10)]
        [InlineData("{var x = 0 var res = 0 for x = 1+1 x < 2 x = x+1 { res = x } res}", 0)]
        [InlineData("{var x = 0 var res = 0 for x = 1 x < 10 x = x+x { res = x } res}", 8)]
        public void EvaluationTests_GetText_RoundTrips(string text, object value)
        {
            AssertValue(text, value);
        }

        private static void AssertValue(string text, object value)
        {
            var variables = new Dictionary<VariableSymbol, object>();

            var compilation = new Compilation(SyntaxTree.Parse(text));
            var evaluatedResult = compilation.Evaluate(variables);

            Assert.Empty(evaluatedResult.Diagnostics);
            Assert.Equal(value, evaluatedResult.Value);
        }

        [Fact]
        public void Evaluation_IfStatement_Reports_Convert()
        {
            var text = @"
                {
                    var x = 0
                    if [10]
                        x = 10
                }";
            var diagnostic = $"Cannot convert from {typeof(int)} to {typeof(bool)}";

            AssertHasDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluation_WhileStatement_Reports_Convert()
        {
            var text = @"
                {
                    var x = 0
                    while [10]
                        x = 10
                }";
            var diagnostic = $"Cannot convert from {typeof(int)} to {typeof(bool)}";

            AssertHasDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluation_VariableDeclaration_Reports_Redeclaration()
        {
            var text = @"
                {
                    var x = 10
                    var y = 22
                    {
                        var x = 10
                    }
                    var [x] = 5
                }";
            var diagnostic = @"Variable name 'x' already declared";

            AssertHasDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluation_BlockStatement_Reports_NoInifiniteLoop()
        {
            var text = @"{
                        [)]
                        []";
            var diagnostic = @"
                    Unexpected token <CloseParenthesisToken>, expected <IdentifierToken>
                    Unexpected token <EOFToken>, expected <CloseBraceToken>";

            AssertHasDiagnostics(text, diagnostic);
        }


        [Fact]
        public void Evaluation_Name_Reports_NoErrorForInsertedToken()
        {
            var text = @"[]";
            var diagnostic = @"Unexpected token <EOFToken>, expected <IdentifierToken>";

            AssertHasDiagnostics(text, diagnostic);
        }


        [Fact]
        public void Evaluation_Name_Reports_Undefined()
        {
            var text = @"[x] + 10";
            var diagnostic = @"Undefined variable name 'x'";

            AssertHasDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluation_Assignment_Reports_Undefined()
        {
            var text = @"[x] = 10";
            var diagnostic = @"Undefined variable name 'x'";

            AssertHasDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluation_Assignment_Reports_Readonly()
        {
            var text = @"
                        {
                            let x = 10
                            x [=] 0
                        }";
            var diagnostic = @"Cannot assign to readonly varaible 'x'";

            AssertHasDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluation_Assignment_Reports_Convert()
        {
            var text = @"
                        {
                            var x = 2
                            x = [true]
                        }";
            var diagnostic = $"Cannot convert from {typeof(bool)} to {typeof(int)}";

            AssertHasDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluation_UnaryOperator_Undefined()
        {
            var text = @"[+]true";
            var diagnostic = $"Unary operator '+' is not defined for type {typeof(bool)}";

            AssertHasDiagnostics(text, diagnostic);
        }

        [Fact]
        public void Evaluation_BinaryOperator_Undefined()
        {
            var text = @"true[+]false";
            var diagnostic = $"Binary operator '+' is not defined for types {typeof(bool)} and {typeof(bool)}";

            AssertHasDiagnostics(text, diagnostic);
        }

        private static void AssertHasDiagnostics(string text, string diagnosticText)
        {
            var annotatedText = AnnotatedText.Parse(text);
            var syntaxTree = SyntaxTree.Parse(annotatedText.Text);

            var compilation = new Compilation(syntaxTree);
            var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

            var expectedDiagnostics = AnnotatedText.UnindentLines(diagnosticText);
            if (annotatedText.Spans.Length != expectedDiagnostics.Count)
            {
                throw new Exception("ERROR:Must mark as many spans as there are expected diagnostics");
            }

            Assert.Equal(expectedDiagnostics.Count, result.Diagnostics.Length);

            for (var i = 0; i < expectedDiagnostics.Count; ++i)
            {
                var expectedMessage = expectedDiagnostics[i];
                var actualMessage = result.Diagnostics[i].Message;

                Assert.Equal(expectedMessage, actualMessage);

                var expectedSpan = annotatedText.Spans[i];
                var actualSpan = result.Diagnostics[i].Span;

                Assert.Equal(expectedSpan, actualSpan);
            }
        }
    }
}