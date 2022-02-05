using System.IO;
using System.Runtime.CompilerServices;

[Serializable]
public class InvalidInputException : LotusException
{
    public InvalidInputException(LocationRange range, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            message: $"Could not process invalid input. No more info available. ({Path.GetFileNameWithoutExtension(callerPath)}.{caller})",
            range,
            caller,
            callerPath
        ) { }

    public InvalidInputException(string input, LocationRange range, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"Could not process input '{input}' {GetRangeString(range)}",
            range,
            caller,
            callerPath
        ) { }

    public InvalidInputException(string input, string context, LocationRange range, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base($"Could not process input '{input}' {GetRangeString(range)} {context}.",
            range,
            caller,
            callerPath
        ) { }

    public InvalidInputException(string input, string context, string reason, LocationRange range, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base($"Could not process input '{input}' {GetRangeString(range)} {context} {reason}.",
            range,
            caller,
            callerPath
        ) { }
}