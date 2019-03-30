namespace MCompiler.CodeAnalysis.Syntax
{
    using System.Collections.Generic;

    internal sealed class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private int _position;
        private DiagnosticBag _diagnostics = new DiagnosticBag();
        public Parser(string text)
        {
            Lexer lexer = new Lexer(text);

            var tokens = new List<SyntaxToken>();
            SyntaxToken token;

            do
            {
                token = lexer.Lex();
                if (token.Kind != SyntaxKind.WhiteSpaceToken
                && token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EOFToken);

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

        public DiagnosticBag Diagnostics => _diagnostics;

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
            _diagnostics.ReportUnexpectedToken(Current.span, Current.Kind, kind);
            return new SyntaxToken(kind, Current.Position, null, null);
        }
        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var eof = MatchToken(SyntaxKind.EOFToken);
            return new SyntaxTree(Diagnostics, expression, eof);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseAssignmentExpression();
        }
        private ExpressionSyntax ParseAssignmentExpression()
        {

            if (Peek(0).Kind == SyntaxKind.IndentifierToken
            && Peek(1).Kind == SyntaxKind.EqualsToken)
            {
                var indentifierToken = NextToken();
                var operatorToken = NextToken();
                var right = ParseAssignmentExpression();
                return new AssignmentExpressionSyntax(indentifierToken, operatorToken, right);
            }

            return ParseBinaryExpression();
        }

        private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            var unaryOperaorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
            if (unaryOperaorPrecedence != 0 && unaryOperaorPrecedence >= parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseBinaryExpression(unaryOperaorPrecedence);
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
                var right = ParseBinaryExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenthesisToken:
                    {
                        var left = NextToken();
                        var expression = ParseExpression();
                        var right = MatchToken(SyntaxKind.CloseParenthesisToken);
                        return new ParenthesizedExpression(left, expression, right);
                    }

                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    {
                        var value = Current.Kind == SyntaxKind.TrueKeyword;
                        return new LiteralExpressionSyntax(literalToken: NextToken(), value: value);
                    }
                case SyntaxKind.IndentifierToken:
                    {
                        var indentifierToken = NextToken();
                        return new NameExpressionSyntax(indentifierToken);
                    }
                default:
                    var numberToken = MatchToken(SyntaxKind.NumberToken);
                    return new LiteralExpressionSyntax(numberToken);
            }
        }
    }
}