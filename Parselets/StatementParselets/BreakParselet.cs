public sealed class BreakParslet : IStatementParslet<BreakNode>
{
    public BreakNode Parse(StatementParser parser, Token breakToken) {
        if (!(breakToken is Token breakKeyword && breakKeyword == "break")) {
            throw Logger.Fatal(new InvalidCallException(breakToken.Location));
        }

        return new BreakNode(breakKeyword);
    }
}