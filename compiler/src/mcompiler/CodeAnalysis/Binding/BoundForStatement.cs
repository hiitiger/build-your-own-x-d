namespace MCompiler.CodeAnalysis.Binding
{
    internal sealed class BoundForStatement: BoundLoopStatement
    {
        public BoundExpression Initializer;
        public BoundExpression Condition;
        public BoundExpression Loop;
        public BoundStatement Body;
        public override BoundNodeKind Kind => BoundNodeKind.ForStatement;

        public BoundForStatement(BoundExpression initializer, BoundExpression condition, BoundExpression loop, BoundStatement body, BoundLabel breakLabel, BoundLabel continueLabel)
        : base(breakLabel, continueLabel)
        {
            this.Initializer = initializer;
            this.Condition = condition;
            this.Loop = loop;
            this.Body = body;
        }
    }
}