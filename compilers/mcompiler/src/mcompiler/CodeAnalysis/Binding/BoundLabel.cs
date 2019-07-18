namespace MCompiler.CodeAnalysis
{
    public class BoundLabel
    {
        internal BoundLabel(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString() => Name;
    }
}