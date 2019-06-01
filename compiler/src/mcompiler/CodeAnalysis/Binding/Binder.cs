using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MCompiler.CodeAnalysis.Symbol;
using MCompiler.CodeAnalysis.Syntax;

namespace MCompiler.CodeAnalysis.Binding
{
    internal class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private BoundScope _scope;
        public DiagnosticBag Diagnostics => _diagnostics;

        public Binder(BoundScope parent)
        {
            _scope = new BoundScope(parent);
        }

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, CompilationUnitSyntax syntax)
        {
            var parentScope = CreateParentScopes(previous);
            var binder = new Binder(parentScope);
            var statement = binder.BindStatement(syntax.Statement);
            var variables = binder._scope.GetDeclaredVariables();
            var diagnostics = binder.Diagnostics.ToImmutableArray();
            if (previous != null)
                diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);

            return new BoundGlobalScope(previous, diagnostics, variables, statement);
        }

        private static BoundScope CreateParentScopes(BoundGlobalScope previous)
        {
            var stack = new Stack<BoundGlobalScope>();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }

            BoundScope parent = null;
            while (stack.Count > 0)
            {
                var prev = stack.Pop();
                var scope = new BoundScope(parent);
                foreach (var v in prev.Variables)
                    scope.TryDeclare(v);

                parent = scope;
            }

            return parent;
        }


        public BoundStatement BindStatement(StatementSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.BlockStatement:
                    return BindBlockstatement((BlockStatementSyntax)syntax);
                case SyntaxKind.ExpressionStatement:
                    return BindExpressionStatement((ExpressionStatementSyntax)syntax);
                case SyntaxKind.VariableDeclarationStatement:
                    return BindVariableDeclarationStatement((VariableDeclarationStatementSyntax)syntax);
                case SyntaxKind.IfStatement:
                    return BindIfStatement((IfStatementSyntax)syntax);
                case SyntaxKind.WhileStatement:
                    return BindWhileStatement((WhileStatementSyntax)syntax);
                case SyntaxKind.ForStatement:
                    return BindForStatement((ForStatementSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundForStatement BindForStatement(ForStatementSyntax syntax)
        {
            _scope = new BoundScope(_scope);
            var initializer = BindExpression(syntax.Initializer);
            var condition = BindExpression(syntax.Condition);
            var loop = BindExpression(syntax.Loop);
            var body = BindStatement(syntax.Body);
            _scope = _scope.Parent;
            return new BoundForStatement(initializer, condition, loop, body);
        }

        private BoundWhileStatement BindWhileStatement(WhileStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            var body = BindStatement(syntax.Body);
            return new BoundWhileStatement(condition, body);
        }

        private BoundIfStatement BindIfStatement(IfStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            var statement = BindStatement(syntax.ThenStatement);
            var elseStatement = syntax.ElseClause == null ? null : BindStatement(syntax.ElseClause.ElseStatement);
            return new BoundIfStatement(condition, statement, elseStatement);
        }

        private BoundVariableDeclarationStatement BindVariableDeclarationStatement(VariableDeclarationStatementSyntax syntax)
        {
            var isReadonly = syntax.KeywordToken.Kind == SyntaxKind.LetKeyword;
            var initializer = BindExpression(syntax.Initializer);
            var variable = BindVariable(syntax.IdentifierToken, isReadonly, initializer.Type);

            return new BoundVariableDeclarationStatement(variable, initializer);
        }

        private VariableSymbol BindVariable(SyntaxToken identifierToken, bool isReadonly, TypeSymbol type)
        {
            var name = identifierToken.Text ?? "?";
            var variable = new VariableSymbol(name, isReadonly, type);

            var declared = !identifierToken.IsMissing;

            if (declared && !_scope.TryDeclare(variable))
            {
                _diagnostics.ReportVariableAlreadyDeclared(identifierToken.Span, name);
            }

            return variable;
        }

        private BoundBlockStatement BindBlockstatement(BlockStatementSyntax syntax)
        {
            var statements = ImmutableArray.CreateBuilder<BoundStatement>();
            _scope = new BoundScope(_scope);
            foreach (var statementSyntax in syntax.Statements)
            {
                var statement = BindStatement(statementSyntax);
                statements.Add(statement);
            }
            _scope = _scope.Parent;
            return new BoundBlockStatement(statements.ToImmutable());
        }

        private BoundExpressionStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            var expression = BindExpression(syntax.Expression);
            return new BoundExpressionStatement(expression);
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax, TypeSymbol type)
        {
            var expression = BindExpression(syntax);
            if (expression.Type != type
                && expression.Type != TypeSymbol.Error
                && type != TypeSymbol.Error)
                _diagnostics.ReportCannotConvert(syntax.Span, expression.Type, type);

            return expression;
        }

        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                case SyntaxKind.ParenthesizedExpression:
                    return BindParenthesizedExpression((ParenthesizedExpression)syntax);
                case SyntaxKind.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case SyntaxKind.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpression syntax)
        {
            return BindExpression(syntax.Expression);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            if (!_scope.TryLookup(name, out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
                return boundExpression;
            }

            if (variable.IsReadonly)
            {
                _diagnostics.ReportCannotAssign(syntax.EqualsToken.Span, name);
                return boundExpression;
            }

            if (boundExpression.Type != variable.Type)
            {
                _diagnostics.ReportCannotConvert(syntax.Expression.Span, boundExpression.Type, variable.Type);
                return boundExpression;
            }

            return new BoundAssignmentExpression(variable, boundExpression);
        }

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            if (syntax.IdentifierToken.IsMissing)
                return new BoundErrorExpression();

            if (!_scope.TryLookup(name, out VariableSymbol variable))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
                return new BoundErrorExpression();
            }

            return new BoundVariableExpression(variable);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);

            if (boundOperand.Type == TypeSymbol.Error)
                return new BoundErrorExpression();


            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);
                return new BoundErrorExpression();
            }
            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeftOperand = BindExpression(syntax.Left);
            var boundRightOperand = BindExpression(syntax.Right);

            if (boundLeftOperand.Type == TypeSymbol.Error || boundRightOperand.Type == TypeSymbol.Error)
                return new BoundErrorExpression();

            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeftOperand.Type, boundRightOperand.Type);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeftOperand.Type, boundRightOperand.Type);
                return new BoundErrorExpression();
            }
            return new BoundBinaryExpression(boundLeftOperand, boundOperator, boundRightOperand);
        }
    }
}