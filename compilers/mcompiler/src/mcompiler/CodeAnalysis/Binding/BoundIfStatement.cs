namespace MCompiler.CodeAnalysis.Binding
{
    internal class BoundIfStatement : BoundStatement
    {
        public BoundExpression Condition;
        public BoundStatement Statement;
        public BoundStatement ElseStatement;

        public BoundIfStatement(BoundExpression condition, BoundStatement statement, BoundStatement elseStatement)
        {
            this.Condition = condition;
            this.Statement = statement;
            this.ElseStatement = elseStatement;
        }

        public override BoundNodeKind Kind => BoundNodeKind.IfStatement;
    }
}