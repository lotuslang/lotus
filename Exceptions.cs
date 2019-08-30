using System;
using System.Collections.Generic;

[System.Serializable]
public class UnexpectedTokenException : System.Exception
{
    public UnexpectedTokenException(string message, Token token)
        : base($"{token.Location} : Unexpected {token.Kind} ({token.Representation}). {message}") { }
    public UnexpectedTokenException(Token token, params TokenKind[] expected)
        : base($"{token.Location} : Unexpected {token.Kind} ({token.Representation}). Expected `{String.Join("`, or `", new List<TokenKind>(expected))}`") { }
    public UnexpectedTokenException(Token token, string expected)
        : base($"{token.Location} : Unexpected {token.Kind} ({token.Representation}). Expected `{expected}`") { }
    public UnexpectedTokenException(Token token, string context, params TokenKind[] expected)
        : base ($"{token.Location} : Unexpected {token.Kind} ({token.Representation}) {context}. Expected `{String.Join("', or '", new List<TokenKind>(expected))}`") { }
    public UnexpectedTokenException(Token token, string context, string expected)
        : base ($"{token.Location} : Unexpected {token.Kind} ({token.Representation}) {context}. Expected `{expected}`") { }
}

[System.Serializable]
public class InvalidOperationException : System.Exception
{
    public InvalidOperationException(string message, Token token)
        : base($"{token.Location} : Incompatible operation. {message}") { }
    public InvalidOperationException(OperationNode op, StatementNode operand)
        : base($"{op.Token.Location} : Cannot apply operator '{op.Representation} to operand of type '{operand.GetFriendlyName()}'.") { }
    public InvalidOperationException(OperationNode op, StatementNode operand, params string[] expected)
        : base($"{op.Token.Location} : Cannot apply operator '{op.Representation} to operand of type '{operand.GetFriendlyName()}'."
        + $"'{op.Representation}' can only be applied to operand of type '{String.Join("', or '", new List<string>(expected))}'") { }
    public InvalidOperationException(OperationNode op, StatementNode operand1, StatementNode operand2)
        : base($"{op.Token.Location} : Cannot apply operator '{op.Representation} to operands of type '{operand1.GetFriendlyName()}' and '{operand2.GetFriendlyName()}'.") { }
    public InvalidOperationException(OperationNode op, StatementNode operand1, StatementNode operand2, params string[] expected)
        : base($"{op.Token.Location} : Cannot apply operator '{op.Representation} to operands of type '{operand1.GetFriendlyName()}' and '{operand2.GetFriendlyName()}'."
        + $"'{op.Representation}' can only be applied to operand of type '{String.Join("', or '", new List<string>(expected))}'") { }
}

[System.Serializable]
public class UnknownNameException : System.Exception
{
    public UnknownNameException() { }
}