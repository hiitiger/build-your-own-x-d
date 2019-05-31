using System;
using System.Collections;
using System.Collections.Generic;
using MCompiler.CodeAnalysis.Syntax;
using MCompiler.CodeAnalysis.Text;

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

        internal void ReportVariableAlreadyDeclared(TextSpan span, string name)
        {
            var message = $"Variable name '{name}' already declared";
            Report(span, message);
        }

        internal void ReportCannotConvert(TextSpan span, Type type1, Type type2)
        {
            var message = $"Cannot convert from {type1} to {type2}";
            Report(span, message);
        }

        internal void ReportCannotAssign(TextSpan span, string name)
        {
            var message = $"Cannot assign to readonly varaible '{name}'";
            Report(span, message);
        }

        internal void ReportUnterminatedString(TextSpan span)
        {
            var message = $"Unterminated string literal";
            Report(span, message);
        }
    }
}