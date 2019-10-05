using System;
using System.Collections.Generic;

public enum Precedence {
    Comma = 0,
    Parenthesis = Comma,
    Curly = Comma,
    Assignment = 1,
    Declaration = Assignment,
    LogicalOR = 2,
    LogicalAND = 3,
    Equal = 4,
    NotEqual = Equal,
    LessThan = 5,
    GreaterThan = LessThan,
    LessThanOrEqual = LessThan,
    GreaterThanOrEqual = LessThan,
    Addition = 6,
    Substraction = Addition,
    Multiplication = 7,
    Division = Multiplication,
    Modulo = Multiplication,
    Power = 8,
    Unary = 9,
    Access = 10,
    FuncCall = 11,
    Array = FuncCall,
}

public static class Constants
{
    public static readonly List<string> keywords = new List<string>() {
        "var",
        "new",
        "def",
        "return",
        "true",
        "false",
    };

    static readonly Dictionary<ExpressionKind, IPrefixParselet> PreParselets = new Dictionary<ExpressionKind, IPrefixParselet>();

    static readonly Dictionary<ExpressionKind, IInfixParselet> InAndPostParselets = new Dictionary<ExpressionKind, IInfixParselet>();

    static Constants() {
        RegisterPrefix(ExpressionKind.Number, new NumberParselet());
        RegisterPrefix(ExpressionKind.String, new StringParselet());
        RegisterPrefix(ExpressionKind.Identifier, new IdentifierParselet());
        RegisterPrefix(ExpressionKind.Boolean, new BoolParselet());
        RegisterPrefix(ExpressionKind.LeftParen, new GroupParenParselet());
        RegisterPrefix(ExpressionKind.Plus, new PrefixOperatorParselet());
        RegisterPrefix(ExpressionKind.Minus, new PrefixOperatorParselet());
        RegisterPrefix(ExpressionKind.Not, new PrefixOperatorParselet());

        RegisterInfix(ExpressionKind.Plus, Precedence.Addition, "Add");
        RegisterInfix(ExpressionKind.Minus, Precedence.Substraction, "Sub");
        RegisterInfix(ExpressionKind.Multiply, Precedence.Multiplication, "Mul");
        RegisterInfix(ExpressionKind.Divide, Precedence.Division, "Div");
        RegisterInfix(ExpressionKind.Power, Precedence.Power, "Pow");
        RegisterInfix(ExpressionKind.Eq, Precedence.Equal, "Eq");
        RegisterInfix(ExpressionKind.NotEq, Precedence.NotEqual, "NotEq");
        RegisterInfix(ExpressionKind.Less, Precedence.LessThan, "Less");
        RegisterInfix(ExpressionKind.LessOrEq, Precedence.LessThanOrEqual, "LessOrEq");
        RegisterInfix(ExpressionKind.Greater, Precedence.GreaterThan, "Greater");
        RegisterInfix(ExpressionKind.GreaterOrEq, Precedence.GreaterThanOrEqual, "GreaterOrEq");
        RegisterInfix(ExpressionKind.Access, Precedence.Access, "Access");
        RegisterInfix(ExpressionKind.Assignment, Precedence.Assignment, "Assign");

        InAndPostParselets.Add(ExpressionKind.ArrayAccess, new ArrayAccessParselet());
        InAndPostParselets.Add(ExpressionKind.LeftParen, new FuncCallParselet());
    }

    static void RegisterPrefix(ExpressionKind kind, IPrefixParselet parselet)
        => PreParselets.Add(kind, parselet);

    static void RegisterInfix(ExpressionKind kind, Precedence precedence, string operationType)
        => InAndPostParselets.Add(kind, new BinaryOperatorParselet(precedence, operationType));

    static void RegisterPostfix(ExpressionKind kind, string operationType)
        => InAndPostParselets.Add(kind, new PostfixOperatorParselet(operationType));

    public static ExpressionKind GetExpressionKind(this Token token) {
        switch (token.Kind) {
            case TokenKind.ident:
                return ExpressionKind.Identifier;
            case TokenKind.number:
                return ExpressionKind.Number;
            case TokenKind.@bool:
                return ExpressionKind.Boolean;
            case TokenKind.@string:
            case TokenKind.complexString:
                return ExpressionKind.String;
        }

        switch (token.Representation) {
            case "+":
                return ExpressionKind.Plus;
            case "-":
                return ExpressionKind.Minus;
            case "*":
                return ExpressionKind.Multiply;
            case "/":
                return ExpressionKind.Divide;
            case "^":
                return ExpressionKind.Power;
            case "!":
                return ExpressionKind.Not;
            case "==":
                return ExpressionKind.Eq;
            case "!=":
                return ExpressionKind.NotEq;
            case "<":
                return ExpressionKind.Less;
            case ">":
                return ExpressionKind.Greater;
            case "<=":
                return ExpressionKind.LessOrEq;
            case ">=":
                return ExpressionKind.GreaterOrEq;
            case "&&":
                return ExpressionKind.And;
            case "||":
                return ExpressionKind.Or;
            case "^^":
                return ExpressionKind.Xor;
            case "++":
                return ExpressionKind.Increment;
            case "--":
                return ExpressionKind.Decrement;
            case "(":
                return ExpressionKind.LeftParen;
            case "[":
                return ExpressionKind.ArrayAccess;
            case ".":
                return ExpressionKind.Access;
            case "=":
                return ExpressionKind.Assignment;
        }

        return ExpressionKind.NotAnExpr;
    }

    public static IPrefixParselet GetPrefixParselet(ExpressionKind kind)
        => PreParselets[kind];

    public static IPrefixParselet GetPrefixParselet(Token token)
        => PreParselets[token.GetExpressionKind()];

    public static IInfixParselet GetOperatorParselet(ExpressionKind kind)
        => InAndPostParselets[kind];

    public static IInfixParselet GetOperatorParselet(Token token)
        => InAndPostParselets[token.GetExpressionKind()];

    public static bool HasPrefixParselet(this ExpressionKind kind)
        => PreParselets.ContainsKey(kind);

    public static bool HasOperatorParselet(this ExpressionKind kind)
        => InAndPostParselets.ContainsKey(kind);
}