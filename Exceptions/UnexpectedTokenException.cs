using System;
using System.IO;
using System.Runtime.CompilerServices;

[Serializable]
public class UnexpectedTokenException : LotusException
{
    public UnexpectedTokenException(Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"Unexpected token encountered at location {location}. No more info available. "
                + $"(from {Path.GetFileNameWithoutExtension(callerPath)}.{caller})",
            location,
            caller,
            callerPath
        ) { }

    public UnexpectedTokenException(string message, Token token, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"{token.Location} : Unexpected {token.Kind} ({token.Representation}) at location {token.Location}. {message}. ",
            token.Location,
            caller,
            callerPath
        ) { }

    public UnexpectedTokenException(Token token, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "", params TokenKind[] expected)
        : base(
            $"{token.Location} : Unexpected {token.Kind} ({token.Representation}) at location {token.Location}. "
                + $"Expected `{String.Join("`, or `", expected)}`.",
            token.Location,
            caller,
            callerPath
        ) { }

    public UnexpectedTokenException(Token token, string expected, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"{token.Location} : Unexpected {token.Kind} ({token.Representation}) at location {token.Location}. Expected {expected}. ",
            token.Location,
            caller,
            callerPath
        ) { }

    public UnexpectedTokenException(Token token, string context, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "", params TokenKind[] expected)
        : base (
            $"{token.Location} : Unexpected {token.Kind} ({token.Representation}) at location {token.Location} {context}. "
                + $"Expected `{String.Join("`, or `", expected)}`.",
            token.Location,
            caller,
            callerPath
        ) { }

    public UnexpectedTokenException(Token token, string context, string expected, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base (
            $"{token.Location} : Unexpected {token.Kind} ({token.Representation}) at location {token.Location} {context}. "
                + $"Expected {expected}.",
            token.Location,
            caller,
            callerPath
        ) { }
}