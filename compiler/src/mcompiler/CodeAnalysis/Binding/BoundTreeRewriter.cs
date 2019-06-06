using System;
using System.Collections.Immutable;

namespace MCompiler.CodeAnalysis.Binding
{
    internal abstract class BoundTreeRewriter
    {
        public virtual BoundNode RewriteNode(BoundNode node)
        {
            if (node is BoundStatement s)
                return RewriteStatement(s);
            else if (node is BoundExpression e)
                return RewriteExpression(e);
            else
                throw new Exception($"Unexpected node: {node.Kind}");
        }

        public virtual BoundStatement RewriteStatement(BoundStatement node)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.BlockStatement:
                    return RewriteBlockStatement((BoundBlockStatement)node);
                case BoundNodeKind.ExpressionStatement:
                    return RewriteExpressionStatement((BoundExpressionStatement)node);
                case BoundNodeKind.VariableDeclarationStatement:
                    return RewriteVariableDeclarationStatement((BoundVariableDeclarationStatement)node);
                case BoundNodeKind.IfStatement:
                    return RewriteIfStatement((BoundIfStatement)node);
                case BoundNodeKind.WhileStatement:
                    return RewriteWhileStatement((BoundWhileStatement)node);
                case BoundNodeKind.ForStatement:
                    return RewriteForStatement((BoundForStatement)node);
                case BoundNodeKind.DoWhileStatement:
                    return RewriteDoWhileStatement((BoundDoWhileStatement)node);
                case BoundNodeKind.LabelStatement:
                    return RewriteLabelStatement((BoundLabelStatement)node);
                case BoundNodeKind.GotoStatement:
                    return RewriteGotoStatement((BoundGotoStatement)node);
                case BoundNodeKind.ConditionalGotoStatement:
                    return RewriteConditionalGotoStatement((BoundConditionalGotoStatement)node);
                default:
                    throw new Exception($"Unexpected node: {node.Kind}");
            }
        }

        protected virtual BoundStatement RewriteDoWhileStatement(BoundDoWhileStatement node)
        {
            var body = RewriteStatement(node.Body);
            var condition = RewriteExpression(node.Condition);
            if (body == node.Body && condition == node.Condition)
                return node;

            return new BoundDoWhileStatement(body, condition);
        }

        protected virtual BoundStatement RewriteConditionalGotoStatement(BoundConditionalGotoStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            if (condition == node.Condition)
                return node;
            return new BoundConditionalGotoStatement(node.Label, condition, node.JumpIfFalse);
        }

        protected virtual BoundStatement RewriteGotoStatement(BoundGotoStatement node)
        {
            return node;
        }

        protected virtual BoundStatement RewriteLabelStatement(BoundLabelStatement node)
        {
            return node;
        }

        protected virtual BoundStatement RewriteForStatement(BoundForStatement node)
        {
            var initializer = RewriteExpression(node.Initializer);
            var condition = RewriteExpression(node.Condition);
            var loop = RewriteExpression(node.Loop);
            var body = RewriteStatement(node.Body);
            if (initializer == node.Initializer && condition == node.Condition && loop == node.Loop && body == node.Body)
                return node;
            return new BoundForStatement(initializer, condition, loop, body);
        }

        protected virtual BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            var body = RewriteStatement(node.Body);
            if (condition == node.Condition && body == node.Body)
                return node;
            return new BoundWhileStatement(condition, body);
        }

        protected virtual BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            var statement = RewriteStatement(node.Statement);
            var elseStatement = node.ElseStatement == null ? null : RewriteStatement(node.ElseStatement);
            if (condition == node.Condition && statement == node.Statement && elseStatement == node.ElseStatement)
                return node;
            return new BoundIfStatement(condition, statement, elseStatement);
        }

        protected virtual BoundStatement RewriteVariableDeclarationStatement(BoundVariableDeclarationStatement node)
        {
            var initializer = RewriteExpression(node.Initializer);
            if (initializer == node.Initializer)
                return node;
            return new BoundVariableDeclarationStatement(node.Variable, initializer);
        }

        protected virtual BoundStatement RewriteExpressionStatement(BoundExpressionStatement node)
        {
            var expression = RewriteExpression(node.Expression);
            if (expression == node.Expression)
                return node;
            return new BoundExpressionStatement(expression);
        }

        protected virtual BoundStatement RewriteBlockStatement(BoundBlockStatement node)
        {
            ImmutableArray<BoundStatement>.Builder builder = null;
            for (var i = 0; i < node.Statements.Length; ++i)
            {
                var statement = RewriteStatement(node.Statements[i]);
                if (statement != node.Statements[i])
                {
                    if (builder == null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundStatement>(node.Statements.Length);
                        for (var j = 0; j < i; ++j)
                            builder.Add(node.Statements[j]);
                    }
                }

                if (builder != null)
                    builder.Add(statement);
            }

            if (builder == null)
                return node;
            return new BoundBlockStatement(builder.MoveToImmutable());
        }

        public virtual BoundExpression RewriteExpression(BoundExpression node)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.LiteralExpression:
                    return RewriteLiteralExpression((BoundLiteralExpression)node);
                case BoundNodeKind.UnaryExpression:
                    return RewriteUnaryExpression((BoundUnaryExpression)node);
                case BoundNodeKind.BinaryExpression:
                    return RewriteBinaryExpression((BoundBinaryExpression)node);
                case BoundNodeKind.VariableExpression:
                    return RewriteVariableExpression((BoundVariableExpression)node);
                case BoundNodeKind.AssignmentExpression:
                    return RewriteAssignmentExpression((BoundAssignmentExpression)node);
                case BoundNodeKind.CallExpression:
                    return RewriteCallExpression((BoundCallExpression)node);
                case BoundNodeKind.ConversionExpression:
                    return RewriteConversionExpression((BoundConversionExpression)node);
                case BoundNodeKind.ErrorExpression:
                    return RewriteErrorExpression((BoundErrorExpression)node);
                default:
                    throw new Exception($"Unexpected node: {node.Kind}");
            }
        }

        protected virtual BoundExpression RewriteConversionExpression(BoundConversionExpression node)
        {
            var expression = RewriteExpression(node.Expression);
            if (expression == node.Expression)
                return node;
            return new BoundConversionExpression(node.Type, expression);
        }

        protected virtual BoundExpression RewriteCallExpression(BoundCallExpression node)
        {
            ImmutableArray<BoundExpression>.Builder builder = null;
            for (var i = 0; i < node.Arguments.Length; ++i)
            {
                var expression = RewriteExpression(node.Arguments[i]);
                if (expression != node.Arguments[i])
                {
                    if (builder == null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundExpression>(node.Arguments.Length);
                        for (var j = 0; j < i; ++j)
                            builder.Add(node.Arguments[j]);
                    }
                }

                if (builder != null)
                    builder.Add(expression);
            }

            if (builder == null)
                return node;
            return new BoundCallExpression(node.Function, builder.MoveToImmutable());
        }

        protected virtual BoundExpression RewriteErrorExpression(BoundErrorExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteAssignmentExpression(BoundAssignmentExpression node)
        {
            var expression = RewriteExpression(node.Expression);
            if (expression == node.Expression)
                return node;
            else
                return new BoundAssignmentExpression(node.Variable, expression);
        }

        protected virtual BoundExpression RewriteVariableExpression(BoundVariableExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteBinaryExpression(BoundBinaryExpression node)
        {
            var left = RewriteExpression(node.Left);
            var right = RewriteExpression(node.Right);
            if (left == node.Left && right == node.Right)
                return node;
            else
                return new BoundBinaryExpression(left, node.Op, right);
        }

        protected virtual BoundExpression RewriteUnaryExpression(BoundUnaryExpression node)
        {
            var operand = RewriteExpression(node.Operand);
            if (operand == node.Operand)
                return node;
            else
                return new BoundUnaryExpression(node.Op, operand);
        }

        protected virtual BoundExpression RewriteLiteralExpression(BoundLiteralExpression node)
        {
            return node;
        }
    }
}