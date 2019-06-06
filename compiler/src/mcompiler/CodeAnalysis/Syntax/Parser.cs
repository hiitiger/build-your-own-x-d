namespace MCompiler.CodeAnalysis.Syntax
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using MCompiler.CodeAnalysis.Text;

    internal sealed class Parser
    {
        private readonly ImmutableArray<SyntaxToken> _tokens;
        private int _position;
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        private readonly SourceText _text;

        public Parser(SourceText text)
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
            _text = text;
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
        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var statament = ParseStatement();
            var eof = MatchToken(SyntaxKind.EOFToken);
            return new CompilationUnitSyntax(statament, eof);
        }

        private StatementSyntax ParseStatement()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                    return ParseBlockStatement();
                case SyntaxKind.LetKeyword:
                case SyntaxKind.VarKeyword:
                    return ParseVariableDeclaration();
                case SyntaxKind.IfKeyword:
                    return ParseIfStatement();
                case SyntaxKind.WhileKeyword:
                    return ParseWhileStatement();
                case SyntaxKind.ForKeyword:
                    return ParseForStatement();
                case SyntaxKind.DoKeyword:
                    return ParseDoWhileStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        private DoWhileStatementSyntax ParseDoWhileStatement()
        {
            var doKeyword = MatchToken(SyntaxKind.DoKeyword);
            var statement = ParseBlockStatement();
            var whileKeyword = MatchToken(SyntaxKind.WhileKeyword);
            var condition = ParseExpression();
            return new DoWhileStatementSyntax(doKeyword, statement, whileKeyword, condition);
        }

        private ForStatementSyntax ParseForStatement()
        {
            var keyword = MatchToken(SyntaxKind.ForKeyword);
            var initializer = ParseExpression();
            var condition = ParseExpression();
            var loop = ParseExpression();
            var statement = ParseStatement();
            return new ForStatementSyntax(keyword, initializer, condition, loop, statement);
        }

        private WhileStatementSyntax ParseWhileStatement()
        {
            var keyword = MatchToken(SyntaxKind.WhileKeyword);
            var condition = ParseExpression();
            var body = ParseStatement();
            return new WhileStatementSyntax(keyword, condition, body);
        }

        private VariableDeclarationStatementSyntax ParseVariableDeclaration()
        {
            var expected = Current.Kind == SyntaxKind.LetKeyword ? SyntaxKind.LetKeyword : SyntaxKind.VarKeyword;
            var keyword = MatchToken(expected);
            var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
            var equalsToken = MatchToken(SyntaxKind.EqualsToken);
            var initializer = ParseExpression();
            return new VariableDeclarationStatementSyntax(keyword, identifierToken, equalsToken, initializer);
        }

        private BlockStatementSyntax ParseBlockStatement()
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();

            var openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);
            while (Current.Kind != SyntaxKind.EOFToken
                && Current.Kind != SyntaxKind.CloseBraceToken)
            {
                var startToken = Current;
                var statement = ParseStatement();

                if (Current == startToken)
                    NextToken();
                else
                    statements.Add(statement);
            }

            var closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);

            return new BlockStatementSyntax(openBraceToken, statements.ToImmutable(), closeBraceToken);
        }

        private ExpressionStatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();
            return new ExpressionStatementSyntax(expression);
        }

        private IfStatementSyntax ParseIfStatement()
        {
            var keyword = MatchToken(SyntaxKind.IfKeyword);
            var condition = ParseExpression();
            var statement = ParseStatement();
            var elseClause = ParseElseClause();
            return new IfStatementSyntax(keyword, condition, statement, elseClause);
        }

        private ElseClauseSyntax ParseElseClause()
        {
            if (Current.Kind != SyntaxKind.ElseKeyword)
                return null;
            var keyword = NextToken();
            var statement = ParseStatement();
            return new ElseClauseSyntax(keyword, statement);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseAssignmentExpression();
        }
        private ExpressionSyntax ParseAssignmentExpression()
        {

            if (Peek(0).Kind == SyntaxKind.IdentifierToken
            && Peek(1).Kind == SyntaxKind.EqualsToken)
            {
                var identifierToken = NextToken();
                var operatorToken = NextToken();
                var right = ParseAssignmentExpression();
                return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
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
                case SyntaxKind.StringToken:
                    return ParseStringLiteral();
                case SyntaxKind.IdentifierToken:
                default:
                    return ParseNameOrCallExpression();
            }
        }

        private ExpressionSyntax ParseNumberLiteral()
        {
            var numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }

        private ExpressionSyntax ParseStringLiteral()
        {
            var stringToken = MatchToken(SyntaxKind.StringToken);
            return new LiteralExpressionSyntax(stringToken);
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

        private ExpressionSyntax ParseNameOrCallExpression()
        {
            if(Peek(0).Kind == SyntaxKind.IdentifierToken && Peek(1).Kind == SyntaxKind.OpenParenthesisToken)
                return ParsCallExpression();
            else
                return ParseNameExpression();
        }

        private ExpressionSyntax ParsCallExpression()
        {
            var indentifierToken = MatchToken(SyntaxKind.IdentifierToken);
            var openParenthesisToken = MatchToken(SyntaxKind.OpenParenthesisToken);
            var arguments = ParseArguments();
            var closeParenthesisToken = MatchToken(SyntaxKind.CloseParenthesisToken);
            return new CallExpressionSyntax(indentifierToken, openParenthesisToken, arguments, closeParenthesisToken);
        }

        private SeparatedSyntaxList<ExpressionSyntax> ParseArguments()
        {
            var nodeAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();

            while(Current.Kind != SyntaxKind.CloseParenthesisToken && Current.Kind != SyntaxKind.EOFToken)
            {
                var expression = ParseExpression();
                nodeAndSeparators.Add(expression);

                if (Current.Kind != SyntaxKind.CloseParenthesisToken)
                {
                    var comma = MatchToken(SyntaxKind.CommaToken);
                    nodeAndSeparators.Add(comma);
                }
            }

            return new SeparatedSyntaxList<ExpressionSyntax>(nodeAndSeparators.ToImmutable());
        }

        private ExpressionSyntax ParseNameExpression()
        {
            var indentifierToken = MatchToken(SyntaxKind.IdentifierToken);
            return new NameExpressionSyntax(indentifierToken);
        }
    }
}