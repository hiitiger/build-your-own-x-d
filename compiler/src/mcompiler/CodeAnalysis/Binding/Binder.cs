using System;
using System.Collections.Generic;
using MCompiler.CodeAnalysis.Syntax;

namespace MCompiler.CodeAnalysis.Binding
{
    internal class Binder
    {
        private readonly List<string> _diagnostics = new List<string>();
        public IEnumerable<string> Diagnostics => _diagnostics;
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
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperatorKind = BindUnaryOperatorKind(syntax.OperatorToken.Kind, boundOperand.Type);
            if (boundOperatorKind == null)
            {
                _diagnostics.Add($"Unary operator '{syntax.OperatorToken.Text}' is not defined for type {boundOperand.Type}");
                return boundOperand;
            }
            return new BoundUnaryExpression(boundOperatorKind.Value, boundOperand);
        }

        private BoundUnaryOperatorKind? BindUnaryOperatorKind(SyntaxKind kind, Type operandType)
        {
            if (operandType == typeof(int))
            {
                switch (kind)
                {
                    case SyntaxKind.Plus:
                        return BoundUnaryOperatorKind.Indentity;
                    case SyntaxKind.Minus:
                        return BoundUnaryOperatorKind.Negation;
                    default:
                        throw new Exception($"Unexpected unary operator syntax {kind}");
                }
            }
            else if (operandType == typeof(bool))
            {
                switch (kind)
                {
                    case SyntaxKind.Bang:
                        return BoundUnaryOperatorKind.LogicalNegation;
                    default:
                        throw new Exception($"Unexpected unary operator syntax {kind}");
                }
            }

            return null;
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeftOperand = BindExpression(syntax.Left);
            var boundRightOperand = BindExpression(syntax.Right);
            var boundOperatorKind = BindBinaryOperatorKind(syntax.OperatorToken.Kind, boundLeftOperand.Type, boundRightOperand.Type);
            if (boundOperatorKind == null)
            {
                _diagnostics.Add($"Binary operator '{syntax.OperatorToken.Text}' is not defined for types {boundLeftOperand.Type} and {boundRightOperand.Type}");
                return boundLeftOperand;
            }
            return new BoundBinaryExpression(boundLeftOperand, boundOperatorKind.Value, boundRightOperand);
        }

        private BoundBinaryOperatorKind? BindBinaryOperatorKind(SyntaxKind kind, Type leftType, Type rightType)
        {
            if (leftType == typeof(int) && rightType == typeof(int))
            {
                switch (kind)
                {
                    case SyntaxKind.Plus:
                        return BoundBinaryOperatorKind.Addition;
                    case SyntaxKind.Minus:
                        return BoundBinaryOperatorKind.Subtraction;
                    case SyntaxKind.Star:
                        return BoundBinaryOperatorKind.Multiplication;
                    case SyntaxKind.Slash:
                        return BoundBinaryOperatorKind.Devision;
                    default:
                        throw new Exception($"Unexpected binary operator syntax {kind}");
                }
            }
            else if (leftType == typeof(bool) && rightType == typeof(bool))
            {
                 switch (kind)
                {
                    case SyntaxKind.AmpersandAmpersand:
                        return BoundBinaryOperatorKind.LogicalAnd;
                    case SyntaxKind.PipePipe:
                        return BoundBinaryOperatorKind.LogicalOr;
                    default:
                        throw new Exception($"Unexpected binary operator syntax {kind}");
                }
            }
            else
            {
                return null;
            }
        }
    }
}