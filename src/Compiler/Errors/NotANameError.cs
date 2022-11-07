using System.Runtime.CompilerServices;

using Lotus.Syntax;

namespace Lotus.Error;

public class NotANameError : UnexpectedError, IValued<ValueNode>
{
    private ValueNode _val;
    public required ValueNode Value {
        get => _val;
        init {
            _val = value;
            _loc = _val.Location;
        }
    }

    private LocationRange _loc = LocationRange.NULL;
    public override LocationRange Location => _loc;

    public override string? As { get; init; }

#pragma warning disable CS8618
    public NotANameError(ErrorArea area, [CallerMemberName] string caller = "<unknown-caller>", [CallerFilePath] string callerPath = "")
        : base(area, caller: caller, callerPath: callerPath) { }
}