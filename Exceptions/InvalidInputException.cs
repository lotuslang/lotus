using System;
using System.IO;
using System.Runtime.CompilerServices;

[Serializable]
public class InvalidInputException : Exception
{
    public InvalidInputException([CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base($"Could not process invalid input. No more info available. ({Path.GetFileNameWithoutExtension(callerPath)}.{caller})") { }

    public InvalidInputException(string input, Location location)
        : base($"{location} : Could not process input '{input}'") { }

    public InvalidInputException(string input, string context, Location location)
        : base($"{location} : Could not process input '{input}' {context}.") { }

    public InvalidInputException(string input, string context, string reason, Location location)
        : base($"{location} : Could not process input '{input}' {context} {reason}.") { }
}