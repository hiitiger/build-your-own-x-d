using System;
using System.Collections.Generic;
using System.Linq;

namespace compiler
{
    class Program
    {

        static void PrettyPrint(SyntaxNode node,  string indent = "")
        {
            Console.Write(indent);
            Console.Write(node.Kind);
            if(node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();
            indent += "    ";

            foreach (var child in node.GetChildren()) {
                PrettyPrint(child, indent);
            }
        }

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

                var parser = new Parser(line);
                var expression = parser.Parse();

                var color = Console.BackgroundColor;
                // Console.ForegroundColor = ConsoleColor.DarkGray;

                PrettyPrint(expression);

                // Console.ForegroundColor = color;

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
        EOF,
        NumberExpression,
        BinaryExpression
    }

    class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public override SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
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

    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }

    abstract class ExpressionSyntax : SyntaxNode
    {

    }

    sealed class NumberExpressionSyntax : ExpressionSyntax
    {
        public NumberExpressionSyntax(SyntaxToken numberToken)
        {
            NumberToken = numberToken;
        }
        public override SyntaxKind Kind => SyntaxKind.NumberExpression;

        public SyntaxToken NumberToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken;
        }
    }

    sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            Left = left;
            Right = right;
            OperatorToken = operatorToken;
        }
        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
        public ExpressionSyntax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Right { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }

    class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private int _position;
        public Parser(string text)
        {
            Lexer lexer = new Lexer(text);

            var tokens = new List<SyntaxToken>();
            SyntaxToken token;

            do
            {
                token = lexer.nextToken();
                if (token.Kind != SyntaxKind.WhiteSpace
                && token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EOF);

            _tokens = tokens.ToArray();
        }

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];

            return _tokens[index];
        }

        private SyntaxToken Current => Peek(0);

        private SyntaxToken NextToken()
        {
            var cur = Current;
            _position += 1;
            return cur;
        }
        private SyntaxToken Match(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();
            return new SyntaxToken(kind, Current.Position, null, null);
        }
        public ExpressionSyntax Parse()
        {
            var left = ParsePrimaryExpression();

            while (Current.Kind == SyntaxKind.Plus
                || Current.Kind == SyntaxKind.Minus)
            {
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        public ExpressionSyntax ParsePrimaryExpression()
        {
            var numberToken = Match(SyntaxKind.Number);
            return new NumberExpressionSyntax(numberToken);
        }
    }
}
