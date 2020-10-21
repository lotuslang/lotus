using System;
using System.IO;
using System.Runtime.CompilerServices;

[Serializable]
public class InvalidCallException : InternalErrorException
{
    public InvalidCallException(LocationRange range, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            message: $"The method {caller} (from {Path.GetFileNameWithoutExtension(callerPath)}.{caller}) shouldn't have been called on this input."
                    + " This is a *serious* error",
            range: range
        ) { }

    public InvalidCallException(string message, LocationRange range, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            message: $"The method {caller} (from {Path.GetFileNameWithoutExtension(callerPath)}.{caller}) shouldn't have been called. {message}. "
                    + "This is (generally) a *serious* error",
            range: range
        ) { }
}