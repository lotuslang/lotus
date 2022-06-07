public sealed class NumberLiteralParslet : IPrefixParslet<NumberNode>
{

    private static NumberLiteralParslet _instance = new();
    public static NumberLiteralParslet Instance => _instance;

	private NumberLiteralParslet() : base() { }

    public NumberNode Parse(ExpressionParser parser, Token token) {
        if (token is NumberToken numberToken) {
            return new NumberNode(numberToken);
        }

        throw Logger.Fatal(new InvalidCallException(token.Location));
    }
}
