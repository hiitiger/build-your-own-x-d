using System;
using System.Collections.Generic;
using System.Linq;
using MCompiler.CodeAnalysis.Syntax;

namespace MCompiler.CodeAnalysis.Binding
{
    internal class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private Dictionary<VariableSymbol, object> _variables;

        public Binder(Dictionary<VariableSymbol, object> variables)
        {
            _variables = variables;
        }

        public DiagnosticBag Diagnostics => _diagnostics;
        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                case SyntaxKind.ParenthesizedExpression:
                    return BindParenthesizedExpression((ParenthesizedExpression)syntax);
                case SyntaxKind.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case SyntaxKind.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpression syntax)
        {
            return BindExpression(syntax.Expression);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            var variable = new VariableSymbol(name, boundExpression.Type);
            var existingVariable = _variables.Keys.FirstOrDefault(k => k.Name == name);
            if (existingVariable != null)
                _variables.Remove(existingVariable);

            _variables[variable] = null;

            return new BoundAssignmentExpression(variable, boundExpression);
        }

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var variable = _variables.Keys.FirstOrDefault(k => k.Name == name);
            if (variable == null)
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.span, name);
                return new BoundLiteralExpression(0);
            }

            return new BoundVariableExpression(variable);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.span, syntax.OperatorToken.Text, boundOperand.Type);
                return boundOperand;
            }
            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeftOperand = BindExpression(syntax.Left);
            var boundRightOperand = BindExpression(syntax.Right);
            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeftOperand.Type, boundRightOperand.Type);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.span, syntax.OperatorToken.Text, boundRightOperand.Type, boundRightOperand.Type);
                return boundLeftOperand;
            }
            return new BoundBinaryExpression(boundLeftOperand, boundOperator, boundRightOperand);
        }
    }
}