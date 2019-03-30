using System;
using System.Collections;
using System.Collections.Generic;
using MCompiler.CodeAnalysis.Syntax;

namespace MCompiler.CodeAnalysis
{
    internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();
        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void AddRange(IEnumerable<Diagnostic> diagnostics)
        {
            _diagnostics.AddRange(diagnostics);
        }

        public void Report(TextSpan span, string message)
        {
            var diagnostic = new Diagnostic(span, message);
            _diagnostics.Add(diagnostic);
        }

        public void ReportInvalidNumber(TextSpan textSpan, string text, Type type)
        {
            var message = $"The number {text} isn's valid {type}.";
            Report(textSpan, message);
        }

        public void ReportBadCharacter(int position, char current)
        {
            var message = $"Bad character '{current}'";
            Report(span: new TextSpan(position, 1), message: message);
        }

        public void ReportUnexpectedToken(TextSpan span, SyntaxKind actualKind, SyntaxKind expectedKind)
        {
            var message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>";
            Report(span, message);
        }

        public void ReportUndefinedUnaryOperator(TextSpan span, string text, Type type)
        {
            var message = $"Unary operator '{text}' is not defined for type {type}";
            Report(span, message);
        }

        public void ReportUndefinedBinaryOperator(TextSpan span, string text, Type type1, Type type2)
        {
            var message = $"Binary operator '{text}' is not defined for types {type1} and {type2}";
            Report(span, message);
        }

        internal void ReportUndefinedName(TextSpan span, string name)
        {
           var message = $"Undefined variable name '{name}'";
            Report(span, message);
        }
    }
}