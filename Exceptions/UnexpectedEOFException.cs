using System;
using System.IO;
using System.Runtime.CompilerServices;

[Serializable]
public sealed class UnexpectedEOFException : LotusException
{

    public UnexpectedEOFException(LocationRange range, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
             $"Unexpected EOF encountered {GetRangeString(range)}. No more info available."
            +$"(from {Path.GetFileNameWithoutExtension(callerPath)}.{caller})",
            range,
            caller,
            callerPath
        ) { }

    public UnexpectedEOFException(string message, LocationRange range, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"{range}: Unexpected EOF encountered. {message}.",
            range,
            caller,
            callerPath
        ) { }

    public UnexpectedEOFException(string context, string expected, LocationRange range, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"{range}: Unexpected EOF encountered {context}. Expected {expected}.",
            range,
            caller,
            callerPath
        ) { }
}