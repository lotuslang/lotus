public sealed class NumberLiteralParslet : IPrefixParslet<NumberNode>
{
    public NumberNode Parse(ExpressionParser parser, Token token) {
        if (token is NumberToken numberToken) {
            return new NumberNode(numberToken);
        }

        throw Logger.Fatal(new InvalidCallException(token.Location));
    }
}
