using System.Runtime.CompilerServices;

using Lotus.Syntax;

namespace Lotus.Error;

public class UnexpectedEOFError : UnexpectedError<Token>, ILocalized
{
    public required new LocationRange Location { get; init; } = LocationRange.NULL;

    [SetsRequiredMembers]
    public UnexpectedEOFError([CallerMemberName] string caller = "<unknown-caller>", [CallerFilePath] string callerPath = "")
        : base(ErrorArea.Tokenizer & ErrorArea.Parser, caller, callerPath)
    {
        Value = Token.NULL;
    }

    [SetsRequiredMembers]
    public UnexpectedEOFError(ErrorArea area, [CallerMemberName] string caller = "<unknown-caller>", [CallerFilePath] string callerPath = "")
        : base(area, caller, callerPath) {
        Value = Token.NULL;
    }
}