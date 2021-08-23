public sealed class ContinueParslet : IStatementParslet<ContinueNode>
{
    public ContinueNode Parse(StatementParser parser, Token continueToken) {
        if (!(continueToken is Token continueKeyword && continueKeyword == "continue")) {
            throw Logger.Fatal(new InvalidCallException(continueToken.Location));
        }

        return new ContinueNode(continueKeyword);
    }
}