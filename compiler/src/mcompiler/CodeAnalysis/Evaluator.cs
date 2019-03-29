namespace MCompiler.CodeAnalysis
{
    using System;


    public class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            this._root = root;
        }

        public int Evaluate()
        {
            return EvalutateExpression(_root);
        }

        private int EvalutateExpression(ExpressionSyntax root)
        {
            if (root is LiteralExpressionSyntax n)
            {
                return (int)n.LiteralToken.Value;
            }

            if (root is BinaryExpressionSyntax b)
            {
                var left = EvalutateExpression(b.Left);
                var right = EvalutateExpression(b.Right);
                if (b.OperatorToken.Kind == SyntaxKind.Plus)
                {
                    return left + right;
                }
                else if (b.OperatorToken.Kind == SyntaxKind.Minus)
                {
                    return left - right;
                }
                else if (b.OperatorToken.Kind == SyntaxKind.Star)
                {
                    return left * right;
                }
                else if (b.OperatorToken.Kind == SyntaxKind.Slash)
                {
                    return left / right;
                }
                else
                    throw new Exception($"Unexpected binary operator {b.OperatorToken.Kind}");
            }

            if (root is ParenthesizedExpression p)
            {
                return EvalutateExpression(p.Expression);
            }

            throw new Exception($"Unexpected node {root.Kind}");
        }
    }

}