using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MCompiler.CodeAnalysis.Binding
{
    public abstract class BoundNode
    {
        public abstract BoundNodeKind Kind { get; }



        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                this.WriteTo(writer);
                return writer.ToString();
            }
        }

    }
}