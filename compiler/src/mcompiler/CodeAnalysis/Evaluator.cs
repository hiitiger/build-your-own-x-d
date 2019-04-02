﻿namespace MCompiler.CodeAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MCompiler.CodeAnalysis.Binding;
    using MCompiler.CodeAnalysis.Syntax;

    public class Evaluator
    {
        private readonly BoundExpression _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        public Evaluator(BoundExpression root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            return EvalutateExpression(_root);
        }

        private object EvalutateExpression(BoundExpression root)
        {
            if (root is BoundLiteralExpression n)
            {
                return n.Value;
            }

            if (root is BoundVariableExpression v)
            {
                return _variables[v.Variable];
            }

            if(root is BoundAssignmentExpression a)
            {
                var value = EvalutateExpression(a.Expression);
                _variables[a.Variable] = value;
                return value;
            }

            if (root is BoundUnaryExpression u)
            {
                var operand = EvalutateExpression(u.Operand);
                switch (u.Op.Kind)
                {
                    case BoundUnaryOperatorKind.Indentity:
                        return (int)operand;
                    case BoundUnaryOperatorKind.Negation:
                        return -(int)operand;
                    case BoundUnaryOperatorKind.LogicalNegation:
                        return !(bool)operand;
                    default:
                        throw new Exception($"Unexpected unary operator {u.Op.Kind}");
                }
            }

            if (root is BoundBinaryExpression b)
            {
                var left = EvalutateExpression(b.Left);
                var right = EvalutateExpression(b.Right);
                switch (b.Op.Kind)
                {
                    case BoundBinaryOperatorKind.Addition:
                        return (int)left + (int)right;
                    case BoundBinaryOperatorKind.Subtraction:
                        return (int)left - (int)right;
                    case BoundBinaryOperatorKind.Multiplication:
                        return (int)left * (int)right;
                    case BoundBinaryOperatorKind.Devision:
                        return (int)left / (int)right;
                    case BoundBinaryOperatorKind.LogicalAnd:
                        return (bool)left && (bool)right;
                    case BoundBinaryOperatorKind.LogicalOr:
                        return (bool)left || (bool)right;
                    case BoundBinaryOperatorKind.Equals:
                        return object.Equals(left, right);
                    case BoundBinaryOperatorKind.NotEquals:
                        return !object.Equals(left, right);
                    default:
                        throw new Exception($"Unexpected binary operator {b.Op.Kind}");
                }
            }

            throw new Exception($"Unexpected node {root.Kind}");
        }
    }
}