using System.Runtime.CompilerServices;

using Lotus.Text;

namespace Lotus.Error;

public class InvalidCallError : LotusError, ILocalized
{
    public LocationRange Location { get; init; } = LocationRange.NULL;

    public InvalidCallError(ErrorArea area, [CallerMemberName] string caller = "<unknown-caller>", [CallerFilePath] string callerPath = "")
        : base(area, caller: caller, callerPath: callerPath) { }
    public InvalidCallError(ErrorArea area, LocationRange loc, [CallerMemberName] string caller = "<unknown-caller>", [CallerFilePath] string callerPath = "")
        : base(area, caller: caller, callerPath: callerPath)
    {
            Location = loc;
    }
}