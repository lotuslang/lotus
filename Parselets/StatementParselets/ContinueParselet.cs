public sealed class ContinueParslet : IStatementParslet<ContinueNode>
{

    private static ContinueParslet _instance = new();
    public static ContinueParslet Instance => _instance;

	private ContinueParslet() : base() { }

    public ContinueNode Parse(StatementParser parser, Token continueToken) {
        if (!(continueToken is Token continueKeyword && continueKeyword == "continue")) {
            throw Logger.Fatal(new InvalidCallException(continueToken.Location));
        }

        return new ContinueNode(continueKeyword);
    }
}