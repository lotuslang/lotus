using System;
using System.Collections.Generic;

[System.Serializable]
public class UnexpectedTokenException : System.Exception
{
    public UnexpectedTokenException(string message, Token token) : base($"{token.Location} : Unexpected {token.Kind} ({token.Representation}). " + message) { }
    public UnexpectedTokenException(Token token, params TokenKind[] expected)
        : base($"{token.Location} : Unexpected {token.Kind} ({token.Representation}). Expected {String.Join(", or ", new List<TokenKind>(expected))}") { }
    public UnexpectedTokenException(Token token, string expected)
        : base($"{token.Location} : Unexpected {token.Kind} ({token.Representation}). Expected {expected}") { }
    public UnexpectedTokenException(Token token, string context, params TokenKind[] expected)
        : base ($"{token.Location} : Unexpected {token.Kind} ({token.Representation}) {context}. Expected '{String.Join("', or '", new List<TokenKind>(expected))}'") { }
    public UnexpectedTokenException(Token token, string context, string expected)
        : base ($"{token.Location} : Unexpected {token.Kind} ({token.Representation}) {context}. Expected '{expected}'") { }
}