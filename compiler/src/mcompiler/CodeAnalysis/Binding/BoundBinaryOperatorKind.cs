namespace MCompiler.CodeAnalysis.Binding
{
    internal enum BoundBinaryOperatorKind
    {
        Addition,
        Subtraction,
        Multiplication,
        Devision,
        LogicalAnd,
        LogicalOr,

        Equals,
        NotEquals,
        Less,
        LessEquals,
        Greater,
        GreaterEquals
    }
}