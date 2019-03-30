namespace MCompiler.CodeAnalysis
{
    using System;
    using MCompiler.CodeAnalysis.Binding;
    using MCompiler.CodeAnalysis.Syntax;

    public class Evaluator
    {
        private readonly BoundExpression _root;

        public Evaluator(BoundExpression root)
        {
            _root = root;
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

            if (root is BoundUnaryExpression u)
            {
                var operand = (int)EvalutateExpression(u.Operand);
                if (u.OperatorKind == BoundUnaryOperatorKind.Indentity)
                    return operand;
                else if (u.OperatorKind == BoundUnaryOperatorKind.Negation)
                    return -operand;
                else
                    throw new Exception($"Unexpected unary operator {u.OperatorKind}");
            }

            if (root is BoundBinaryExpression b)
            {
                var left = (int)EvalutateExpression(b.Left);
                var right = (int)EvalutateExpression(b.Right);
                if (b.OperatorKind == BoundBinaryOperatorKind.Addition)
                {
                    return left + right;
                }
                else if (b.OperatorKind == BoundBinaryOperatorKind.Subtraction)
                {
                    return left - right;
                }
                else if (b.OperatorKind == BoundBinaryOperatorKind.Multiplication)
                {
                    return left * right;
                }
                else if (b.OperatorKind == BoundBinaryOperatorKind.Devision)
                {
                    return left / right;
                }
                else
                    throw new Exception($"Unexpected binary operator {b.OperatorKind}");
            }

            throw new Exception($"Unexpected node {root.Kind}");
        }
    }

}