using System;
using System.IO;
using System.Runtime.CompilerServices;

[Serializable]
public class InternalErrorException : LotusException
{
    public InternalErrorException(LocationRange range, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
             $"Internal error {GetRangeString(range)}. "
            +$"Stopped in {Path.GetFileNameWithoutExtension(callerPath)}.{caller}."
            + "No more info. If it happens again, *please* fill an issue on the project's github repo.",
            range,
            caller,
            callerPath
        ) { }

    public InternalErrorException(string message, LocationRange range, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
    : base(
         $"Internal error {GetRangeString(range)}. "
        + message + ". "
        +$"Stopped in {Path.GetFileNameWithoutExtension(callerPath)}.{caller}."
        + "If it happens again, *please* fill an issue on the project's github repo.",
        range,
        caller,
        callerPath
    ) { }

    public InternalErrorException(string context, string reason, LocationRange range, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
             $"Internal error {GetRangeString(range)} {context} {reason}. "
            +$"Stopped in {Path.GetFileNameWithoutExtension(callerPath)}.{caller}."
            + "No more info. If it happens again, *please* fill an issue on the project's github repo.",
            range,
            caller,
            callerPath
        ) { }

}