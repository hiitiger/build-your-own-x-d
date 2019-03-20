using System;

namespace compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }

                var lexer = new Lexer(line);
                while (true)
                {
                    var token = lexer.nextToken();
                    if (token.Kind == SyntaxKind.EOF)
                        break;

                    Console.Write($"{token.Kind}: '{token.Text}' ");

                    if (token.Value != null)
                        Console.Write($"{token.Value}");

                    Console.WriteLine();
                }
            }
        }
    }

    enum SyntaxKind
    {
        Number,
        WhiteSpace,
        Plus,
        Minus,
        Multiply,
        Devide,
        OpenParen,
        CloseParan,
        BadToken,
        EOF
    }

    class SyntaxToken
    {
        public SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }

        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }
    }

    class Lexer
    {
        private readonly string _text;
        private int _position = 0;
        public Lexer(string text)
        {
            _text = text;
        }

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

                int.TryParse(text, out var value);
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
                return new SyntaxToken(SyntaxKind.Multiply, _position++, "*", null);
            if (Current == '/')
                return new SyntaxToken(SyntaxKind.Devide, _position++, "/", null);
            if (Current == '(')
                return new SyntaxToken(SyntaxKind.OpenParen, _position++, "(", null);
            if (Current == ')')
                return new SyntaxToken(SyntaxKind.CloseParan, _position++, ")", null);

            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }

    }
}
