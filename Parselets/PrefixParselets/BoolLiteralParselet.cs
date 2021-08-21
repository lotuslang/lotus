public sealed class BoolLiteralParslet : IPrefixParslet<BoolNode>
{
    public BoolNode Parse(ExpressionParser parser, Token token) {
        if (token is not BoolToken boolToken) {
            throw Logger.Fatal(new InvalidCallException(token.Location));
        }

        return new BoolNode(boolToken.Value, boolToken);
    }
}
