using System;
using System.IO;
using System.Runtime.CompilerServices;

[Serializable]
public class InternalErrorException : Exception
{
    public InternalErrorException([CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base($"Internal error. Stopped in {Path.GetFileNameWithoutExtension(callerPath)}.{caller}. No more info. If it happens again, please fill an issue on the project's github repo.")
    { }

    public InternalErrorException(string message, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base($"Internal error. Message : {message}. Stopped in {Path.GetFileNameWithoutExtension(callerPath)}.{caller}. If it happens again, please fill an issue on the project's github repo.")
    { }

    public InternalErrorException(string context, Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base($"{location} : Internal error {context}. Stopped in {Path.GetFileNameWithoutExtension(callerPath)}.{caller}. No more info. If it happens again, please fill an issue on the project's github repo.")
    { }

    public InternalErrorException(string context, string reason, Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base($"{location} : Internal error {context} {reason}. Stopped in {Path.GetFileNameWithoutExtension(callerPath)}.{caller}. No more info. If it happens again, please fill an issue on the project's github repo.")
    { }

}