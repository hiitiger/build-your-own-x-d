using System;
using System.Collections;
using System.Collections.Generic;
using MCompiler.CodeAnalysis.Symbols;
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

        public void ReportUndefinedUnaryOperator(TextSpan span, string text, TypeSymbol type)
        {
            var message = $"Unary operator '{text}' is not defined for type {type}";
            Report(span, message);
        }

        public void ReportUndefinedBinaryOperator(TextSpan span, string text, TypeSymbol type1, TypeSymbol type2)
        {
            var message = $"Binary operator '{text}' is not defined for types {type1} and {type2}";
            Report(span, message);
        }

        internal void ReportUndefinedName(TextSpan span, string name)
        {
            var message = $"Undefined variable name '{name}'";
            Report(span, message);
        }

        internal void ReportSymbolAlreadyDeclared(TextSpan span, string name)
        {
            var message = $"Symbol '{name}' already declared";
            Report(span, message);
        }

        internal void ReportParameterAlreadyDeclared(TextSpan span, string name)
        {
            var message = $"Parameter '{name}' already declared";
            Report(span, message);
        }

        internal void ReportCannotConvert(TextSpan span, TypeSymbol fromType, TypeSymbol toType)
        {
            var message = $"Cannot convert from {fromType} to {toType}";
            Report(span, message);
        }

        internal void XXX_ReportFunctionsAreUnsupported(TextSpan span, string text)
        {
           var message = $"Function '{text}' with return values are unsupported";
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

        internal void ReportAllPathsMustReturn(TextSpan span)
        {
            var message = $"Function does not return from all paths";
            Report(span, message);
        }

        internal void ReportUndefinedFunction(TextSpan span, string name)
        {
            var message = $"Undefined function '{name}'";
            Report(span, message);
        }

        internal void ReportWrongArgumentCount(TextSpan span, string name, int expectedCount, int count)
        {
            var message = $"Function '{name}' requires {expectedCount} arguments but was given {count}";
            Report(span, message);
        }

        internal void ReportWrongArgumentType(TextSpan span, string name, TypeSymbol expectedType, TypeSymbol actualType)
        {
            var message = $"Function '{name}' requires type {expectedType} but was given type {actualType}";
            Report(span, message);
        }

        internal void ReportExpressionMustHaveValue(TextSpan span)
        {
            var message = "Expression must have a value";
            Report(span, message);
        }

        internal void ReportUndefinedType(TextSpan span, string text)
        {
            var message = $"Undefined type '{text}'";
            Report(span, message);
        }

        internal void ReportCannotConvertImplicit(TextSpan span, TypeSymbol fromType, TypeSymbol toType)
        {
            var message = $"Cannot convert from {fromType} to {toType}, an explicit version exist, are you missing a cast?";
            Report(span, message);
        }

        internal void ReportInvalidBreakContinue(TextSpan span, string text)
        {
            var message = $"Invalid '{text}' keyword, not inside loop";
            Report(span, message);
        }

        internal void ReportInvalidReturnExpression(TextSpan span, string name)
        {
            var message = $"Invalid return type for function '{name}'";
            Report(span, message);
        }

        internal void ReportMissingReturnExpression(TextSpan span, string name, TypeSymbol type)
        {
            var message = $"Missing return type '{type}' for function '{name}'";
            Report(span, message);
        }

        internal void ReportInvalidReturn(TextSpan span)
        {
            var message = $"Invalid 'return' keyword, not in function scope";
            Report(span, message);
        }
    }
}