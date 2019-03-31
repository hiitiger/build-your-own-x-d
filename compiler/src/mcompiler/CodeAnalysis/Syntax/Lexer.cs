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
                return new SyntaxToken(SyntaxKind.EOFToken, _position, "\0", null);
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
                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                while (char.IsWhiteSpace(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);

                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
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
                    return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
                case '*':
                    return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
                case '/':
                    return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);
                case '&':
                    {
                        if (Lookahead == '&')
                        {
                            _position += 2;
                            return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, start, "&&", null);
                        }
                        break;
                    }
                case '|':
                    {
                        if (Lookahead == '|')
                        {
                            _position += 2;
                            return new SyntaxToken(SyntaxKind.PipePipeToken, start, "||", null);
                        }
                        break;
                    }
                case '=':
                    {
                        if (Lookahead == '=')
                        {
                            _position += 2;
                            return new SyntaxToken(SyntaxKind.EqualsEqualsToken, start, "==", null);
                        }
                        else
                        {
                            _position += 1;
                            return new SyntaxToken(SyntaxKind.EqualsToken, start, "=", null);
                        }
                    }
                case '!':
                    {
                        if (Lookahead == '=')
                        {
                            _position += 2;
                            return new SyntaxToken(SyntaxKind.BangEqualsToken, start, "!=", null);
                        }
                        else
                        {
                            _position += 1;
                            return new SyntaxToken(SyntaxKind.BangToken, start, "!", null);
                        }
                    }
            }

            _diagnostics.ReportBadCharacter(_position, Current);
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }

    }

}