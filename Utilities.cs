using System;
using System.Collections;
using System.Collections.Generic;

public static class Utilities
{
    public static readonly string[] keywords = new string[] {
        "var",
        "new",
        "def",
        "return",
        "true",
        "false",
    };

    public static readonly string[] internalFunctions = new string[] {
        "print",
    };

    static readonly Dictionary<ExpressionKind, IPrefixParselet> PreParselets = new Dictionary<ExpressionKind, IPrefixParselet>();

    static readonly Dictionary<ExpressionKind, IInfixParselet> InAndPostParselets = new Dictionary<ExpressionKind, IInfixParselet>();


    static Utilities() {
        RegisterPrefix(ExpressionKind.Number, new NumberLiteralParselet());
        RegisterPrefix(ExpressionKind.String, new StringLiteralParselet());
        RegisterPrefix(ExpressionKind.Identifier, new IdentifierParselet());
        RegisterPrefix(ExpressionKind.Boolean, new BoolLiteralParselet());
        RegisterPrefix(ExpressionKind.LeftParen, new LeftParenParselet());
        RegisterPrefix(ExpressionKind.Plus, new PrefixOperatorParselet("Pos"));
        RegisterPrefix(ExpressionKind.Minus, new PrefixOperatorParselet("Neg"));
        RegisterPrefix(ExpressionKind.Not, new PrefixOperatorParselet("Not"));
        RegisterPrefix(ExpressionKind.Increment, new PrefixOperatorParselet("Incr"));
        RegisterPrefix(ExpressionKind.Decrement, new PrefixOperatorParselet("Decr"));
        RegisterPrefix(ExpressionKind.Array, new ArrayLiteralParselet());
        RegisterPrefix(ExpressionKind.New, new ObjectCreationParselet());

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

        RegisterPostfix(ExpressionKind.Increment, "Incr");
        RegisterPostfix(ExpressionKind.Decrement, "Decr");

        InAndPostParselets.Add(ExpressionKind.Array, new ArrayAccessParselet());
        InAndPostParselets.Add(ExpressionKind.LeftParen, new FuncCallParselet());
    }

    static void RegisterPrefix(ExpressionKind kind, IPrefixParselet parselet)
        => PreParselets.Add(kind, parselet);

    static void RegisterInfix(ExpressionKind kind, Precedence precedence, string operationType)
        => InAndPostParselets.Add(kind, new BinaryOperatorParselet(precedence, operationType));

    static void RegisterPostfix(ExpressionKind kind, string operationType)
        => InAndPostParselets.Add(kind, new PostfixOperatorParselet(operationType));

    public static ExpressionKind GetExpressionKind(this Token token) {

        if (token == null) return ExpressionKind.NotAnExpr;

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
                return ExpressionKind.Array;
            case ".":
                return ExpressionKind.Access;
            case "=":
                return ExpressionKind.Assignment;
            case "new":
                return ExpressionKind.New;
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

    public static bool IsPrefixParselet(this ExpressionKind kind)
        => PreParselets.ContainsKey(kind);

    public static bool IsOperatorParselet(this ExpressionKind kind)
        => InAndPostParselets.ContainsKey(kind);

    public static bool IsName(ValueNode node) {
        if (node is IdentNode) return true;

        if (node is OperationNode op && op.OperationType == "binaryAccess") {
            if (op.Operands.Count != 2) return false;

            // we only need to check the left-hand operand, because we know the right-hand operand
            // is an IdentNode, because an access operation is defined as :
            //  access-operation :
            //      value '.' identifier
            // hence, the left-hand side
            return IsName(op.Operands[0]);
        }

        return false;
    }
}