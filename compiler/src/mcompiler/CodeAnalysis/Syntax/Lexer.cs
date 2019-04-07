namespace MCompiler.CodeAnalysis.Syntax
{
    using System.Collections.Generic;
    using MCompiler.CodeAnalysis.Text;

    internal class Lexer
    {
        private readonly SourceText _text;
        private int _position = 0;
        private int _start;
        private SyntaxKind _kind;
        private object _value;

        private DiagnosticBag _diagnostics = new DiagnosticBag();
        public Lexer(SourceText text)
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
            _start = _position;
            _kind = SyntaxKind.BadToken;
            _value = null;

            switch (Current)
            {
                case '\0':
                    _kind = SyntaxKind.EOFToken;
                    break;
                case '+':
                    _kind = SyntaxKind.PlusToken;
                    _position += 1;
                    break;
                case '-':
                    _kind = SyntaxKind.MinusToken;
                    _position += 1;
                    break;
                case '*':
                    _kind = SyntaxKind.StarToken;
                    _position += 1;
                    break;
                case '/':
                    _kind = SyntaxKind.SlashToken;
                    _position += 1;
                    break;
                case '(':
                    _kind = SyntaxKind.OpenParenthesisToken;
                    _position += 1;
                    break;
                case ')':
                    _kind = SyntaxKind.CloseParenthesisToken;
                    _position += 1;
                    break;
                 case '{':
                    _kind = SyntaxKind.OpenBraceToken;
                    _position += 1;
                    break;
                case '}':
                    _kind = SyntaxKind.CloseBraceToken;
                    _position += 1;
                    break;
                case '&':
                    {
                        if (Lookahead == '&')
                        {
                            _position += 2;
                            _kind = SyntaxKind.AmpersandAmpersandToken;
                        }
                        break;
                    }
                case '|':
                    {
                        if (Lookahead == '|')
                        {
                            _position += 2;
                            _kind = SyntaxKind.PipePipeToken;
                        }
                        break;
                    }
                case '=':
                    {
                        if (Lookahead == '=')
                        {
                            _position += 2;
                            _kind = SyntaxKind.EqualsEqualsToken;
                        }
                        else
                        {
                            _position += 1;
                            _kind = SyntaxKind.EqualsToken;
                        }
                    }
                    break;
                case '!':
                    {
                        if (Lookahead == '=')
                        {
                            _position += 2;
                            _kind = SyntaxKind.BangEqualsToken;
                        }
                        else
                        {
                            _position += 1;
                            _kind = SyntaxKind.BangToken;
                        }
                    }
                    break;
                default:
                    if (char.IsDigit(this.Current))
                    {
                        ReadNumberToken();
                    }
                    else if (char.IsWhiteSpace(Current))
                    {
                        ReadWhiteSpace();
                    }
                    else if (char.IsLetter(Current))
                    {
                        ReadIdentifierOrKeyword();
                    }
                    else
                    {
                        _diagnostics.ReportBadCharacter(_position, Current);
                        _position += 1;
                        _kind = SyntaxKind.BadToken;
                    }
                    break;

            }

            var length = _position - _start;
            var text = SyntaxFacts.GetText(_kind);
            if (text == null)
            {
                text = _text.ToString(_start, length);
            }

            return new SyntaxToken(_kind, _start, text, _value);
        }

        private void ReadWhiteSpace()
        {
            while (char.IsWhiteSpace(Current))
            {
                Next();
            }
            _kind = SyntaxKind.WhiteSpaceToken;
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(Current))
            {
                Next();
            }

            var length = _position - _start;
            var text = _text.ToString(_start, length);
            if (!int.TryParse(text, out var value))
            {
                _diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, typeof(int));
            }
            _kind = SyntaxKind.NumberToken;
            _value = value;
        }
        private void ReadIdentifierOrKeyword()
        {
            while (char.IsLetter(Current))
            {
                Next();
            }

            var length = _position - _start;
            var text = _text.ToString(_start, length);
            _kind = SyntaxFacts.GetKeywordKind(text);
        }
    }

}