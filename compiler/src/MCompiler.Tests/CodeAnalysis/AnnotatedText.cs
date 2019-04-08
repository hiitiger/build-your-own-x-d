using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using MCompiler.CodeAnalysis.Text;

namespace MCompiler.Tests.CodeAnalysis
{
    public sealed class AnnotatedText
    {
        public AnnotatedText(string text, ImmutableArray<TextSpan> spans)
        {
            Text = text;
            Spans = spans;
        }

        public string Text { get; }
        public ImmutableArray<TextSpan> Spans { get; }

        public static AnnotatedText Parse(string text)
        {
            text = Unindent(text);

            var textBuilder = new StringBuilder();
            var spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();
            var startStack = new Stack<int>();
            var position = 0;
            foreach (var c in text)
            {
                if (c == '[')
                {
                    startStack.Push(position);
                }
                else if (c == ']')
                {
                    if (startStack.Count == 0)
                        throw new ArgumentException("Too many ']' in text", nameof(text));

                    var start = startStack.Pop();
                    var end = position;
                    var span = TextSpan.FromBounds(start, end);
                    spanBuilder.Add(span);
                }
                else
                {
                    position += 1;
                    textBuilder.Append(c);
                }
            }
            if (startStack.Count != 0)
                throw new ArgumentException("Missing ']' in text", nameof(text));

            return new AnnotatedText(textBuilder.ToString(), spanBuilder.ToImmutable());
        }

        private static string Unindent(string text)
        {
            var lines = UnindentLines(text);
            return string.Join(Environment.NewLine, lines);
        }

        public static List<string> UnindentLines(string text)
        {
            var lines = new List<string>();
            using (var stringReader = new StringReader(text))
            {
                string line;
                while ((line = stringReader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            var minIndentLine = lines.Where(x => x.Trim().Length != 0)
                                    .Aggregate((agg, next) => next.Length - next.TrimStart().Length < agg.Length - agg.TrimStart().Length ? next : agg);
            var minIndent = minIndentLine.Length - minIndentLine.TrimStart().Length;

            lines = lines.Select(x => x.Trim().Length == 0 ? string.Empty : x.Substring(minIndent)).ToList();

            while (lines.Count > 0 && lines[0].Length == 0)
                lines.RemoveAt(0);
            while (lines.Count > 0 && lines[lines.Count - 1].Length == 0)
                lines.RemoveAt(lines.Count - 1);
            return lines;
        }
    }
}