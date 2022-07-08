public sealed class ContinueParslet : IStatementParslet<ContinueNode>
{
    public static readonly ContinueParslet Instance = new();

    public ContinueNode Parse(StatementParser parser, Token continueToken) {
        if (continueToken is not Token continueKeyword || continueKeyword != "continue") {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, continueToken.Location));
        }

        return new ContinueNode(continueKeyword);
    }
}