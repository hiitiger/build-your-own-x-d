using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MCompiler.CodeAnalysis.Text
{
    public sealed class SourceText
    {
        private SourceText(string text)
        {
            Text = text;
            Lines = ParseLines(this, text);
        }

        private static ImmutableArray<TextLine> ParseLines(SourceText sourceText, string text)
        {
            var result = ImmutableArray.CreateBuilder<TextLine>();

            var lineStart = 0;
            var position = 0;
            while (position < text.Length)
            {
                var lineBreakWidth = GetLineBreakWidth(text, position);
                if (lineBreakWidth == 0)
                {
                    position += 1;
                }
                else
                {
                    AddTextLine(sourceText, result, lineStart, position, lineBreakWidth);
                    position += lineBreakWidth;
                    lineStart = position;
                }
            }

            if (position >= lineStart)
                AddTextLine(sourceText, result, lineStart, position, 0);

            return result.ToImmutable();

            void AddTextLine(SourceText s, ImmutableArray<TextLine>.Builder r, int ls, int lp, int lb)
            {
                var length = lp - ls;
                var lengthIncludingLineBreak = lp - ls + lb;
                var line = new TextLine(s, ls, length, lengthIncludingLineBreak);
                r.Add(line);
            }
        }

        private static int GetLineBreakWidth(string text, int i)
        {
            var c = text[i];
            var l = i + 1 >= text.Length ? '\0' : text[i + 1];
            if (c == '\r' && l == '\n')
                return 2;
            if (c == '\r' || c == '\n')
                return 1;
            return 0;
        }

        public string Text { get; }
        public ImmutableArray<TextLine> Lines { get; }

        public int GetLineIndex(int position)
        {
            var lower = 0;
            var upper = Lines.Length - 1;
            while (lower <= upper)
            {
                var index = (lower + upper) / 2;
                var start = Lines[index].Start;
                var end = Lines[index].End;

                if (start > position)
                {
                    upper = index - 1;
                }
                else if (end <= position)
                {
                    lower = index + 1;
                }
                else
                {
                    return index;
                }
            }

            return lower - 1;
        }

        public static SourceText From(string text)
        {
            return new SourceText(text);
        }

        public override string ToString() => Text;
        public string ToString(int start, int length) => Text.Substring(start, length);
        public string ToString(TextSpan span) => ToString(span.Start, span.Length);

        public char this[int index] => Text[index];
        public int Length => Text.Length;
    }
}