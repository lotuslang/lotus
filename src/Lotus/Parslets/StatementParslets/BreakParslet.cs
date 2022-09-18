public sealed class BreakParslet : IStatementParslet<BreakNode>
{
    public static readonly BreakParslet Instance = new();

    public BreakNode Parse(StatementParser parser, Token breakToken) {
        Debug.Assert(breakToken == "break");

        return new BreakNode(breakToken);
    }
}