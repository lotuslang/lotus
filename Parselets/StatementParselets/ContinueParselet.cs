public sealed class ContinueParslet : IStatementParslet<ContinueNode>
{

    private static ContinueParslet _instance = new();
    public static ContinueParslet Instance => _instance;

	private ContinueParslet() : base() { }

    public ContinueNode Parse(StatementParser parser, Token continueToken) {
        if (continueToken is not Token continueKeyword || continueKeyword != "continue") {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, continueToken.Location));
        }

        return new ContinueNode(continueKeyword);
    }
}