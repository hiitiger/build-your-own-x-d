namespace MCompiler.CodeAnalysis.Binding
{
    internal class BoundConditionalGotoStatement : BoundStatement
    {
        public BoundConditionalGotoStatement(BoundLabel label, BoundExpression condition, bool jumpIfFalse = false)
        {
            Label = label;
            Condition = condition;
            JumpIfFalse = jumpIfFalse;
        }
        public override BoundNodeKind Kind => BoundNodeKind.ConditionalGotoStatement;

        public BoundLabel Label { get; }
        public BoundExpression Condition { get; }
        public bool JumpIfFalse { get; }
    }
}