using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MCompiler.CodeAnalysis.Binding;

namespace MCompiler.CodeAnalysis.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private int _labelCount = 0;
        public Lowerer()
        {
        }

        private BoundLabel GenerateLabel()
        {
            var label = $"label{++_labelCount}";
            return new BoundLabel(label);
        }

        public static BoundBlockStatement Lower(BoundStatement statement)
        {
            var lowerer = new Lowerer();
            var result = lowerer.RewriteStatement(statement);
            return Flatten(result);
        }

        private static BoundBlockStatement Flatten(BoundStatement statement)
        {
            var builder = ImmutableArray.CreateBuilder<BoundStatement>();
            var stack = new Stack<BoundStatement>();
            stack.Push(statement);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current is BoundBlockStatement block)
                {
                    foreach (var s in block.Statements.Reverse())
                        stack.Push(s);
                }
                else
                    builder.Add(current);
            }

            return new BoundBlockStatement(builder.ToImmutable());
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            var initializer = new BoundExpressionStatement(node.Initializer);
            var loop = new BoundExpressionStatement(node.Loop);
            var whileBlock = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(node.Body, loop));
            var whileStatement = new BoundWhileStatement(node.Condition, whileBlock);
            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(initializer, whileStatement));
            return RewriteStatement(result);
        }

        protected override BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            if (node.ElseStatement == null)
            {
                var endLabel = GenerateLabel();
                var gotoFalse = new BoundConditionalGotoStatement(endLabel, node.Condition, true);
                var endLabelStatement = new BoundLabelStatement(endLabel);
                var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(gotoFalse, node.Statement, endLabelStatement));
                return RewriteStatement(result);
            }
            else
            {
                var elseLabel = GenerateLabel();
                var endLabel = GenerateLabel();
                var gotoFalse = new BoundConditionalGotoStatement(elseLabel, node.Condition, true);
                var gotoEnd = new BoundGotoStatement(endLabel);
                var elseLabelStatement = new BoundLabelStatement(elseLabel);
                var endLabelStatement = new BoundLabelStatement(endLabel);

                var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
                    gotoFalse, node.Statement, gotoEnd, elseLabelStatement, node.ElseStatement, endLabelStatement));
                return RewriteStatement(result);
            }
        }

        protected override BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            var checkLabel = GenerateLabel();
            var continueLabel = GenerateLabel();
            var endLabel = GenerateLabel();
            var gotoCheck = new BoundGotoStatement(checkLabel);
            var continueLabelStatement = new BoundLabelStatement(continueLabel);
            var checkLabelStatement = new BoundLabelStatement(checkLabel);
            var gotoTrue = new BoundConditionalGotoStatement(continueLabel, node.Condition, false);
            var endLabelStatement = new BoundLabelStatement(endLabel);

            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
                    gotoCheck, continueLabelStatement, node.Body, checkLabelStatement, gotoTrue, endLabelStatement));
            return RewriteStatement(result);
        }

        protected override BoundStatement RewriteDoWhileStatement(BoundDoWhileStatement node)
        {
            // do
            //      <body>
            // while <condition>
            //
            // ----->
            //
            // continue:
            // <body>
            // gotoTrue <condition> continue
            var continueLabel = GenerateLabel();

            var continueLabelStatement = new BoundLabelStatement(continueLabel);
            var gotoTrue = new BoundConditionalGotoStatement(continueLabel, node.Condition);
            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(continueLabelStatement, node.Body, gotoTrue));
            return RewriteStatement(result);
        }
    }
}