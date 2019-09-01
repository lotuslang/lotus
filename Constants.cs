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
    Access = Unary,
    FuncCall = 10,
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

    /// <summary>
    /// This constant is the equivalent of "null". When a function doesn't return, it will actually set the `#return` variable to this constant.
    /// Variables that are assigned to a non-returning functions will actually be assigned this value.
    /// </summary>
    public static readonly ValueNode NULL = new ValueNode("null", new Token('\0', TokenKind.EOF, null));
}