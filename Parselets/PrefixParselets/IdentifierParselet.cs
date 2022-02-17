public sealed class IdentifierParslet : IPrefixParslet<IdentNode>
{

    private static IdentifierParslet _instance = new();
    public static IdentifierParslet Instance => _instance;

	private IdentifierParslet() : base() { }

    public IdentNode Parse(ExpressionParser parser, Token token) {
        if (token is not IdentToken identToken) {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, token.Location));
        }

        return new IdentNode(identToken);
    }
}
