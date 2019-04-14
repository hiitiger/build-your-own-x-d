using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MCompiler.CodeAnalysis.Binding
{
    public abstract class BoundNode
    {
        public abstract BoundNodeKind Kind { get; }

        public IEnumerable<BoundNode> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (BoundNode)property.GetValue(this);
                    if (child != null)
                        yield return child;
                }
                else if (typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                {
                    var values = (IEnumerable<BoundNode>)property.GetValue(this);
                    foreach (var child in values)
                    {
                        if (child != null)
                            yield return child;
                    }
                }
            }
        }

        public IEnumerable<(string Name, object Value)> GetProperties()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.Name == nameof(Kind)
                || property.Name == nameof(BoundBinaryExpression.Op)
                || property.Name == nameof(BoundBinaryExpression.OperatorKind))
                    continue;

                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType)
                || typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                {
                    continue;
                }

                var value = property.GetValue(this);
                if (value != null)
                    yield return (property.Name, value);
            }
        }

        public void WriteTo(TextWriter writer)
        {
            PrettyPrint(writer, this);
        }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }

        public static void PrettyPrint(TextWriter writer, BoundNode node, string indent = "", bool isLast = true)
        {
            /*
            ├───
            │
            └───
            ─
            */
            bool isConsole = writer == Console.Out;

            var marker = isLast ? "└───" : "├───";
            writer.Write(indent);

            if (isConsole)
                Console.ForegroundColor = ConsoleColor.DarkGray;
            writer.Write(marker);

            if (isConsole)
                Console.ForegroundColor = GetColor(node);
            var text = GetText(node);
            writer.Write(text);

            var isFirstProperty = false;
            foreach (var pro in node.GetProperties())
            {
                if (isFirstProperty)
                {
                    isFirstProperty = false;
                    writer.Write(" ");
                }
                else
                {
                    if (isConsole)
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    writer.Write(", ");
                }

                if (isConsole)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(pro.Name);
                if (isConsole)
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" = ");
                if (isConsole)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(pro.Value);
            }


            if (isConsole)
                Console.ResetColor();

            writer.WriteLine();

            indent += isLast ? "    " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
            {
                PrettyPrint(writer, child, indent, child == lastChild);
            }
        }

        private static string GetText(BoundNode node)
        {
            if (node is BoundBinaryExpression b)
                return b.Op.Kind.ToString() + "Expression";
            if (node is BoundUnaryExpression u)
                return u.Op.Kind.ToString() + "Expression";

            return node.Kind.ToString();
        }

        static private ConsoleColor GetColor(BoundNode node)
        {
            if (node is BoundExpression)
                return ConsoleColor.Blue;
            if (node is BoundStatement)
                return ConsoleColor.Cyan;

            return ConsoleColor.DarkGray;
        }
    }
}