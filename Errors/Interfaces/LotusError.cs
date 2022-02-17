using System;

public abstract class LotusError : Exception
{
    public string Caller { get; }
    public string CallerPath { get; }

    public ErrorArea Area { get; }

    public new string? Message { get; init; }

    public string? ExtraNotes { get; init; }

    protected LotusError(ErrorArea area, string caller, string callerPath) {
        Area = area;
        Caller = caller;
        CallerPath = callerPath;
    }
}