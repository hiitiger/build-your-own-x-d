namespace MCompiler.CodeAnalysis.Text
{
    public sealed class TextLine
    {
        public TextLine(SourceText text, int start, int length, int lengthIncludingLineBreak)
        {
            Text = text;
            Start = start;
            Length = length;
            LengthIncludingLineBreak = lengthIncludingLineBreak;
        }

        public SourceText Text { get; }
        public int Start { get; }
        public int Length { get; }
        public int LengthIncludingLineBreak { get; }
        public int End => Start + LengthIncludingLineBreak;
        public TextSpan Span => new TextSpan(Start, Length);
        public TextSpan SpanIncludingLineBreak => new TextSpan(Start, LengthIncludingLineBreak);

        public override string ToString() => Text.ToString(Start, Length);
        public string Substring(int start, int length) => Text.ToString(start, length);
        public string Substring(TextSpan span) => Substring(span.Start, span.Length);
    }
}