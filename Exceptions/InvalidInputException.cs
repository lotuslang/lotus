using System;
using System.IO;
using System.Runtime.CompilerServices;

[Serializable]
public class InvalidInputException : LotusException
{
    public InvalidInputException(Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            message: $"Could not process invalid input. No more info available. ({Path.GetFileNameWithoutExtension(callerPath)}.{caller})",
            location,
            caller,
            callerPath
        ) { }

    public InvalidInputException(string input, Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"Could not process input '{input}' at location {location}",
            location,
            caller,
            callerPath
        ) { }

    public InvalidInputException(string input, string context, Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base($"Could not process input '{input}' at location {location} {context}.",
            location,
            caller,
            callerPath
        ) { }

    public InvalidInputException(string input, string context, string reason, Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base($"Could not process input '{input}' at location {location} {context} {reason}.",
            location,
            caller,
            callerPath
        ) { }
}