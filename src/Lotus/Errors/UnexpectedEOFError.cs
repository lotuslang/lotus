using System.Runtime.CompilerServices;

public class UnexpectedEOFError : UnexpectedError<Token>, ILocalized
{
    public new LocationRange Location { get; init; } = LocationRange.NULL;

    public UnexpectedEOFError([CallerMemberName] string caller = "<unknown-caller>", [CallerFilePath] string callerPath = "")
        : base(ErrorArea.Tokenizer & ErrorArea.Parser, caller, callerPath)
    {
        Value = Token.NULL;
    }

    public UnexpectedEOFError(ErrorArea area, [CallerMemberName] string caller = "<unknown-caller>", [CallerFilePath] string callerPath = "")
        : base(area, caller, callerPath) {
        Value = Token.NULL;
    }
}