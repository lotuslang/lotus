using System;
using System.IO;
using System.Runtime.CompilerServices;

[Serializable]
public class UnexpectedEOF : Exception
{
    public UnexpectedEOF([CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base($"Unexpected EOF encountered. No more info available. (from {Path.GetFileNameWithoutExtension(callerPath)}.{caller})") { }

    public UnexpectedEOF(string message, Location location)
        : base($"{location} : Unexpected EOF encountered. {message}.") { }

    public UnexpectedEOF(string context, string expected, Location location)
        : base($"{location} : Unexpected EOF encountered {context}. Expected {expected}.") { }
}