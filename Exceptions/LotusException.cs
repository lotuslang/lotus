using System;
using System.IO;
using System.Runtime.CompilerServices;

public class LotusException : Exception
{
    public LocationRange Position;

    public string Caller;

    public string CallerPath;

    public string CallerString => Path.GetFileNameWithoutExtension(CallerPath) + '.' + Caller;

    public LotusException(LocationRange range, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
             $"An error happened {GetRangeString(range)} "
            +$"and originated from {Path.GetFileNameWithoutExtension(callerPath) + '.' + caller}."
        )
    {
        Position = range;
        Caller = caller;
        CallerPath = callerPath;
    }

    public LotusException(string message, LocationRange range, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(Path.GetFileNameWithoutExtension(callerPath) + '.' + caller + " @ " + message)
    {
        Position = range;
        Caller = caller;
        CallerPath = callerPath;
    }

    protected static string GetRangeString(LocationRange range)
        => range.LineLength == 1 && range.ColumnLength == 1 ?
            "at location " + range.GetFirstLocation()
        :   "from location " + range.GetFirstLocation() + " to " + range.GetLastLocation();

    protected static string GetRangeUpString(LocationRange range)
       => range.LineLength == 1 && range.ColumnLength == 1 ?
           "At location " + range.GetFirstLocation()
       : "From location " + range.GetFirstLocation() + " to " + range.GetLastLocation();
}