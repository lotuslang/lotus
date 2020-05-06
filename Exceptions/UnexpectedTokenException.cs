using System;
using System.IO;
using System.Runtime.CompilerServices;

[Serializable]
public class UnexpectedTokenException : Exception
{
    public UnexpectedTokenException([CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base($"Unexpected token encountered. No more info available. (from {Path.GetFileNameWithoutExtension(callerPath)}.{caller})") { }

    public UnexpectedTokenException(string message, Token token)
        : base($"{token.Location} : Unexpected {token.Kind} ({token.Representation}). {message}.") { }

    public UnexpectedTokenException(Token token, params TokenKind[] expected)
        : base($"{token.Location} : Unexpected {token.Kind} ({token.Representation}). Expected `{String.Join("`, or `", expected)}`.") { }

    public UnexpectedTokenException(Token token, string expected)
        : base($"{token.Location} : Unexpected {token.Kind} ({token.Representation}). Expected {expected}.") { }

    public UnexpectedTokenException(Token token, string context, params TokenKind[] expected)
        : base ($"{token.Location} : Unexpected {token.Kind} ({token.Representation}) {context}. Expected `{String.Join("`, or `", expected)}`.") { }

    public UnexpectedTokenException(Token token, string context, string expected)
        : base ($"{token.Location} : Unexpected {token.Kind} ({token.Representation}) {context}. Expected `{expected}`.") { }
}
