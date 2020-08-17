using System;
using System.IO;
using System.Runtime.CompilerServices;

[Serializable]
public class InternalErrorException : LotusException
{
    public InternalErrorException(Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"Internal error at location {location}. Stopped in {Path.GetFileNameWithoutExtension(callerPath)}.{caller}."
                + "No more info. If it happens again, *please* fill an issue on the project's github repo.",
            location,
            caller,
            callerPath
        ) { }

    public InternalErrorException(string message, Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"Internal error at location {location}. {message}. Stopped in {Path.GetFileNameWithoutExtension(callerPath)}.{caller}."
                + "If it happens again, *please* fill an issue on the project's github repo.",
            location,
            caller,
            callerPath
        ) { }

    public InternalErrorException(string context, string reason, Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"Internal error at location {location} {context} {reason}. Stopped in {Path.GetFileNameWithoutExtension(callerPath)}.{caller}."
                + "No more info. If it happens again, *please* fill an issue on the project's github repo.",
            location,
            caller,
            callerPath
        ) { }

}