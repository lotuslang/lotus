using System.Runtime.CompilerServices;

public abstract class UnexpectedError : LotusError, ILocalized, IContextualized
{
    // This is specifically null so that it's faster to check for
    protected LocationRange? loc = null;

    public virtual LocationRange Location {
        get => loc ?? LocationRange.NULL;
        init => loc = value;
    }

    public virtual string? In { get; init; }

    public virtual string? As { get; init; }

    public virtual Union<string, IEnumerable<string>, None> Expected { get; init; } = new None();

    protected UnexpectedError(ErrorArea area, string caller, string callerPath)
        : base(area, caller, callerPath) { }
}

public class UnexpectedError<T> : UnexpectedError, IValued<T>
{
    public T Value { get; init; }

    public override LocationRange Location {
        get => loc ?? (Value as ILocalized)?.Location ?? LocationRange.NULL;
        init => loc = value;
    }

#pragma warning disable CS8618
    public UnexpectedError(ErrorArea area, [CallerMemberName] string caller = "<unknown-caller>", [CallerFilePath] string callerPath = "")
        : base(area, caller: caller, callerPath: callerPath) { }
}