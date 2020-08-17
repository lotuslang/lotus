using System;
using System.IO;
using System.Runtime.CompilerServices;

[Serializable]
public class UnexpectedEOFException : LotusException
{
    public UnexpectedEOFException(Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"Unexpected EOF encountered at location {location}. No more info available."
                + $"(from {Path.GetFileNameWithoutExtension(callerPath)}.{caller})",
            location,
            caller,
            callerPath
        ) { }

    public UnexpectedEOFException(string message, Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"Unexpected EOF encountered at location {location}. {message}.",
            location,
            caller,
            callerPath
        ) { }

    public UnexpectedEOFException(string context, string expected, Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"Unexpected EOF encountered at location {location} {context}. Expected {expected}.",
            location,
            caller,
            callerPath
        ) { }
}