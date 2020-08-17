using System;
using System.IO;
using System.Runtime.CompilerServices;

public class LotusException : Exception
{
    public Location Position;

    public string Caller;

    public string CallerPath;

    public string CallerString => Path.GetFileNameWithoutExtension(CallerPath) + '.' + Caller;

    public LotusException(Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base($"An error happened at position {location} and originated from {Path.GetFileNameWithoutExtension(callerPath) + '.' + caller}.")
    {
        Position = location;
        Caller = caller;
        CallerPath = callerPath;
    }

    public LotusException(string message, Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base($"An error happened at position {location} and originated from {Path.GetFileNameWithoutExtension(callerPath) + '.' + caller} : " + message)
    {
        Position = location;
        Caller = caller;
        CallerPath = callerPath;
    }
}