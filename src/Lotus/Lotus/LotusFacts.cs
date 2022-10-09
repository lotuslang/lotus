using Lotus.Syntax;

namespace Lotus;

public static partial class LotusFacts
{
    public static bool IsPrefixOrValueKind(ExpressionKind kind)
        => ((int)kind & ExpressionKindFlags.PREFIX) == ExpressionKindFlags.PREFIX;
    public static bool IsValueKind(ExpressionKind kind)
        => ((int)kind & ExpressionKindFlags.VALUE) == ExpressionKindFlags.VALUE;
    public static bool IsStrictlyPrefixKind(ExpressionKind kind)
        => IsPrefixOrValueKind(kind) && !IsValueKind(kind);
    public static bool IsInfixOrPostfixKind(ExpressionKind kind)
        => ((int)kind & ExpressionKindFlags.INFIX) == ExpressionKindFlags.INFIX;
    public static bool IsPostfixKind(ExpressionKind kind)
        => ((int)kind & ExpressionKindFlags.POSTFIX) == ExpressionKindFlags.POSTFIX;
    public static bool IsStrictlyInfixKind(ExpressionKind kind)
        => IsInfixOrPostfixKind(kind) && !IsPostfixKind(kind);

    public static bool IsKeyword(string str) => _keywords.Contains(str);

    public static ExpressionKind GetExpressionKind(Token token) {
        switch (token.Kind) {
            case TokenKind.identifier:
                return ExpressionKind.Identifier;
            case TokenKind.number:
                return ExpressionKind.Number;
            case TokenKind.@bool:
                return ExpressionKind.Boolean;
            case TokenKind.@string:
                return ExpressionKind.String;
            case TokenKind.EOF:
                return ExpressionKind.NotAnExpr;
        }

        var str = token.Representation;

        if (!_strToExprKinds.TryGetValue(str, out var kind))
            return ExpressionKind.NotAnExpr;
        return kind;
    }

    public static Precedence GetPrecedence(ExpressionKind kind) {
        if (!_exprToPrecedence.TryGetValue(kind, out var precedence))
            return Precedence.Comma;
        return precedence;
    }

    public static IInfixParslet<ValueNode> GetInfixParslet(ExpressionKind kind) {
        if (!_exprToInfixParslets.TryGetValue(kind, out var parslet))
            Debug.Fail("ExpressionKind '" + kind + "' doesn't correspond to any infix parslet");
        return parslet;
    }

    public static IPrefixParslet<ValueNode> GetPrefixParslet(ExpressionKind kind) {
        if (!_exprToPrefixParslets.TryGetValue(kind, out var parslet))
            Debug.Fail("ExpressionKind '" + kind + "' doesn't correspond to any prefix parslet");
        return parslet;
    }

    public static IPostfixParslet<ValueNode> GetPostfixParslet(ExpressionKind kind) {
        if (!_exprToPostfixParslets.TryGetValue(kind, out var parslet))
            Debug.Fail("ExpressionKind '" + kind + "' doesn't correspond to any postfix parslet");
        return parslet;
    }

    public static bool TryGetStatementParslet(Token token, [NotNullWhen(true)] out IStatementParslet<StatementNode>? parslet) {
        if (token == Token.NULL) {
            parslet = null;
            return false;
        }

        return _strToStmtParslets.TryGetValue(token.Representation, out parslet);
    }

    public static bool TryGetTopLevelParslet(Token token, [NotNullWhen(true)] out ITopLevelParslet<TopLevelNode>? parslet) {
        if (token == Token.NULL) {
            parslet = null;
            return false;
        }

        return _strToTopLevelParslets.TryGetValue(token.Representation, out parslet);
    }
}