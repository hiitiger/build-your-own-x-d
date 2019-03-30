namespace MCompiler.CodeAnalysis.Syntax
{
    using System.Collections.Generic;

    internal sealed class Parser
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
                token = lexer.Lex();
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
        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();
            _diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }
        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var eof = MatchToken(SyntaxKind.EOF);
            return new SyntaxTree(Diagnostics, expression, eof);
        }
        private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            var unaryOperaorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
            if (unaryOperaorPrecedence != 0 && unaryOperaorPrecedence >= parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseExpression(unaryOperaorPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                var precedence = Current.Kind.GetBinaryOperatorPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;
                var operatorToken = NextToken();
                var right = ParseExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }
        // private ExpressionSyntax ParseTerm()
        // {
        //     var left = ParseFactor();

        //     while (Current.Kind == SyntaxKind.Plus
        //         || Current.Kind == SyntaxKind.Minus)
        //     {
        //         var operatorToken = NextToken();
        //         var right = ParseFactor();
        //         left = new BinaryExpressionSyntax(left, operatorToken, right);
        //     }

        //     return left;
        // }

        // private ExpressionSyntax ParseFactor()
        // {
        //     var left = ParsePrimaryExpression();

        //     while (Current.Kind == SyntaxKind.Star
        //         || Current.Kind == SyntaxKind.Slash)
        //     {
        //         var operatorToken = NextToken();
        //         var right = ParsePrimaryExpression();
        //         left = new BinaryExpressionSyntax(left, operatorToken, right);
        //     }

        //     return left;
        // }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenthesis:
                    {
                        var left = NextToken();
                        var expression = ParseExpression();
                        var right = MatchToken(SyntaxKind.CloseParenthesis);
                        return new ParenthesizedExpression(left, expression, right);
                    }

                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    {
                        var value = Current.Kind == SyntaxKind.TrueKeyword;
                        return new LiteralExpressionSyntax(NextToken(), value);
                    }
                default:
                    var numberToken = MatchToken(SyntaxKind.Number);
                    return new LiteralExpressionSyntax(numberToken);
            }
        }
    }
}