using System.Runtime.CompilerServices;

namespace Lotus.Error;

internal class InternalError : LotusError, ILocalized, IContextualized
{
    public LocationRange Location { get; init; } = LocationRange.NULL;

    public string? In { get; init; }

    public InternalError(ErrorArea area = 0, [CallerMemberName] string caller = "<unknown-caller>", [CallerFilePath] string callerPath = "")
        : base(area, caller: caller, callerPath: callerPath) { }
}