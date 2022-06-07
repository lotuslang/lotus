public sealed class BoolLiteralParslet : IPrefixParslet<BoolNode>
{

    private static BoolLiteralParslet _instance = new();
    public static BoolLiteralParslet Instance => _instance;

	private BoolLiteralParslet() : base() { }

    public BoolNode Parse(ExpressionParser parser, Token token) {
        if (token is not BoolToken boolToken) {
            throw Logger.Fatal(new InvalidCallException(token.Location));
        }

        return new BoolNode(boolToken);
    }
}
