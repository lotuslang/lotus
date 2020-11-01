public sealed class BoolLiteralParselet : IPrefixParselet<BoolNode>
{
    public BoolNode Parse(ExpressionParser parser, Token token) {
        if (token is BoolToken boolToken) {
            return new BoolNode(boolToken.Value, boolToken);
        }

        throw Logger.Fatal(new InvalidCallException(token.Location));
    }
}
