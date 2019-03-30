namespace MCompiler.CodeAnalysis.Syntax
{
    using System.Collections.Generic;

    internal class Lexer
    {
        private readonly string _text;
        private int _position = 0;
        private DiagnosticBag _diagnostics = new DiagnosticBag();
        public Lexer(string text)
        {
            _text = text;
        }

        public DiagnosticBag Diagnostics => _diagnostics;
        private char Current => Peek(0);
        public char Lookahead => Peek(1);
        private char Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _text.Length)
            {
                return '\0';
            }
            return _text[index];
        }

        private void Next()
        {
            _position += 1;
        }

        public SyntaxToken Lex()
        {
            if (Current == '\0')
            {
                return new SyntaxToken(SyntaxKind.EOF, _position, "\0", null);
            }
            var start = _position;

            if (char.IsDigit(this.Current))
            {
                while (char.IsDigit(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);
                if (!int.TryParse(text, out var value))
                {
                    _diagnostics.ReportInvalidNumber(new TextSpan(start, length), _text, typeof(int));
                }
                return new SyntaxToken(SyntaxKind.Number, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                while (char.IsWhiteSpace(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);

                return new SyntaxToken(SyntaxKind.WhiteSpace, start, text, null);
            }

            if (char.IsLetter(Current))
            {
                while (char.IsLetter(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);
                var kind = SyntaxFacts.GetKeywordKind(text);

                return new SyntaxToken(kind, start, text, null);
            }

            switch (Current)
            {
                case '+':
                    return new SyntaxToken(SyntaxKind.Plus, _position++, "+", null);
                case '-':
                    return new SyntaxToken(SyntaxKind.Minus, _position++, "-", null);
                case '*':
                    return new SyntaxToken(SyntaxKind.Star, _position++, "*", null);
                case '/':
                    return new SyntaxToken(SyntaxKind.Slash, _position++, "/", null);
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesis, _position++, "(", null);
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesis, _position++, ")", null);
                case '&':
                    {
                        if (Lookahead == '&')
                        {
                            _position += 2;
                            return new SyntaxToken(SyntaxKind.AmpersandAmpersand, start, "&&", null);
                        }
                        break;
                    }
                case '|':
                    {
                        if (Lookahead == '|')
                        {
                            _position += 2;
                            return new SyntaxToken(SyntaxKind.PipePipe, start, "||", null);
                        }
                        break;
                    }
                case '=':
                    {
                        if (Lookahead == '=')
                        {
                            _position += 2;
                            return new SyntaxToken(SyntaxKind.EqualsEquals, start, "==", null);
                        }
                        break;
                    }
                case '!':
                    {
                        if (Lookahead == '=')
                        {
                            _position += 2;
                            return new SyntaxToken(SyntaxKind.BangEquals, start, "!=", null);
                        }
                        else
                            return new SyntaxToken(SyntaxKind.Bang, _position++, "!", null);
                    }
            }

            _diagnostics.ReportBadCharacter(_position, Current);
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }

    }

}