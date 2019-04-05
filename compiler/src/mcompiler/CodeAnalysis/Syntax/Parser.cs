namespace MCompiler.CodeAnalysis.Syntax
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    internal sealed class Parser
    {
        private readonly ImmutableArray<SyntaxToken> _tokens;
        private int _position;
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
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

            _tokens = tokens.ToImmutableArray();
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
            _diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);
            return new SyntaxToken(kind, Current.Position, null, null);
        }
        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var eof = MatchToken(SyntaxKind.EOFToken);
            return new SyntaxTree(Diagnostics.ToImmutableArray(), expression, eof);
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
                    return ParseParenthesizedExpression();
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    return ParseBooleanLiteral();
                case SyntaxKind.NumberToken:
                    return ParseNumberLiteral();
                case SyntaxKind.IndentifierToken:
                default:
                    return ParseNameExpression();
            }
        }

        private ExpressionSyntax ParseNumberLiteral()
        {
            var numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }

        private ExpressionSyntax ParseParenthesizedExpression()
        {
            var left = MatchToken(SyntaxKind.OpenParenthesisToken);
            var expression = ParseExpression();
            var right = MatchToken(SyntaxKind.CloseParenthesisToken);
            return new ParenthesizedExpression(left, expression, right);
        }

        private ExpressionSyntax ParseBooleanLiteral()
        {
            bool isTrue = Current.Kind == SyntaxKind.TrueKeyword;
            var token = isTrue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
            return new LiteralExpressionSyntax(token, isTrue);
        }

        private ExpressionSyntax ParseNameExpression()
        {
            var indentifierToken = MatchToken(SyntaxKind.IndentifierToken);
            return new NameExpressionSyntax(indentifierToken);
        }
    }
}