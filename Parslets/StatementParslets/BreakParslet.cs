public sealed class BreakParslet : IStatementParslet<BreakNode>
{
    public static readonly BreakParslet Instance = new();

    public BreakNode Parse(StatementParser parser, Token breakToken) {
        if (breakToken is not Token breakKeyword || breakKeyword != "break") {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, breakToken.Location));
        }

        return new BreakNode(breakKeyword);
    }
}