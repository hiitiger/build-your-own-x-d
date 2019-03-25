namespace MCompiler.CodeAnalysis
{
    using System.Collections.Generic;

    class Lexer
    {
        private readonly string _text;
        private int _position = 0;
        private List<string> _diagnostics = new List<string>();
        public Lexer(string text)
        {
            _text = text;
        }

        public IEnumerable<string> Diagnostics => _diagnostics;
        private char Current
        {
            get
            {
                if (_position >= _text.Length)
                {
                    return '\0';
                }
                return _text[_position];
            }
        }

        private void Next()
        {
            _position += 1;
        }

        public SyntaxToken nextToken()
        {
            // <numbers>
            // +-*/()
            // <whitespace>

            if (Current == '\0')
            {
                return new SyntaxToken(SyntaxKind.EOF, _position, "\0", null);
            }

            if (char.IsDigit(this.Current))
            {
                var start = _position;
                while (char.IsDigit(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);
                if (!int.TryParse(text, out var value))
                {
                    _diagnostics.Add($"Error: the number {_text} isn't a valid integer");
                }
                return new SyntaxToken(SyntaxKind.Number, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                var start = _position;
                while (char.IsWhiteSpace(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);

                return new SyntaxToken(SyntaxKind.WhiteSpace, start, text, null);
            }

            if (Current == '+')
                return new SyntaxToken(SyntaxKind.Plus, _position++, "+", null);
            if (Current == '-')
                return new SyntaxToken(SyntaxKind.Minus, _position++, "-", null);
            if (Current == '*')
                return new SyntaxToken(SyntaxKind.Star, _position++, "*", null);
            if (Current == '/')
                return new SyntaxToken(SyntaxKind.Slash, _position++, "/", null);
            if (Current == '(')
                return new SyntaxToken(SyntaxKind.OpenParenthesis, _position++, "(", null);
            if (Current == ')')
                return new SyntaxToken(SyntaxKind.CloseParenthesis, _position++, ")", null);

            _diagnostics.Add($"ERROR: bad character input: '{Current}'");
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }

    }

}