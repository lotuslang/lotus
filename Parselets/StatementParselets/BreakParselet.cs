public sealed class BreakParslet : IStatementParslet<BreakNode>
{

    private static BreakParslet _instance = new();
    public static BreakParslet Instance => _instance;

	private BreakParslet() : base() { }

    public BreakNode Parse(StatementParser parser, Token breakToken) {
        if (breakToken is not Token breakKeyword || breakKeyword != "break") {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, breakToken.Location));
        }

        return new BreakNode(breakKeyword);
    }
}