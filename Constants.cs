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
    public static readonly List<string> keywords = new List<string>();

    static Constants() {
        keywords.Add("var");
        keywords.Add("new");
        keywords.Add("def");
        keywords.Add("true");
        keywords.Add("false");
    }
}