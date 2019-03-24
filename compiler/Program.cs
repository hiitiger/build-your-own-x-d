using System;
using System.Collections.Generic;
using System.Linq;

namespace compiler
{
    class Program
    {

        static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            /*
            ├───
            │
            └───
            ─
            */

            var marker = isLast ? "└───" : "├───";
            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);
            if (node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += isLast ? "    " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
            {
                PrettyPrint(child, indent, child == lastChild);
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
                var syntaxTree = parser.Parse();

                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                PrettyPrint(syntaxTree.Root);
                Console.ForegroundColor = color;

                if (syntaxTree.Diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    foreach (var x in syntaxTree.Diagnostics)
                    {
                        Console.WriteLine(x);
                    }
                    Console.ForegroundColor = color;
                }
                else
                {
                    var e = new Evaluator(syntaxTree.Root);
                    var res = e.Evaluate();
                    Console.WriteLine(res);
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
        Star,
        Slash,
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
                return new SyntaxToken(SyntaxKind.OpenParen, _position++, "(", null);
            if (Current == ')')
                return new SyntaxToken(SyntaxKind.CloseParan, _position++, ")", null);

            _diagnostics.Add($"ERROR: bad character input: '{Current}'");
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

    sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken eof)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            Eof = eof;
        }

        public IReadOnlyList<string> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken Eof { get; }
    }

    class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private int _position;
        private List<string> _diagnostics = new List<string>();
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
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];

            return _tokens[index];
        }

        private SyntaxToken Current => Peek(0);

        public IEnumerable<string> Diagnostics => _diagnostics;

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
            _diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }
        public SyntaxTree Parse()
        {
            var expression = ParseTerm();
            var eof = Match(SyntaxKind.EOF);
            return new SyntaxTree(Diagnostics, expression, eof);
        }

        public ExpressionSyntax ParseTerm()
        {
            var left = ParseFactor();

            while (Current.Kind == SyntaxKind.Plus
                || Current.Kind == SyntaxKind.Minus)
            {
                var operatorToken = NextToken();
                var right = ParseFactor();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        public ExpressionSyntax ParseFactor()
        {
            var left = ParsePrimaryExpression();

            while ( Current.Kind == SyntaxKind.Star
                || Current.Kind == SyntaxKind.Slash)
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

    class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            this._root = root;
        }

        public int Evaluate()
        {
            return EvalutateExpression(_root);
        }

        private int EvalutateExpression(ExpressionSyntax root)
        {
            if (root is NumberExpressionSyntax n)
            {
                return (int)n.NumberToken.Value;
            }

            if (root is BinaryExpressionSyntax b)
            {
                var left = EvalutateExpression(b.Left);
                var right = EvalutateExpression(b.Right);
                if (b.OperatorToken.Kind == SyntaxKind.Plus)
                {
                    return left + right;
                }
                else if (b.OperatorToken.Kind == SyntaxKind.Minus)
                {
                    return left - right;
                }
                else if (b.OperatorToken.Kind == SyntaxKind.Star)
                {
                    return left * right;
                }
                else if (b.OperatorToken.Kind == SyntaxKind.Slash)
                {
                    return left / right;
                }
                else
                    throw new Exception($"Unexpected binary operator {b.OperatorToken.Kind}");
            }

            throw new Exception($"Unexpected node {root.Kind}");
        }
    }
}
