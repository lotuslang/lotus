public sealed class IdentifierParslet : IPrefixParslet<IdentNode>
{
    public IdentNode Parse(ExpressionParser parser, Token token) {
        if (token is IdentToken identToken) {
            return new IdentNode(token.Representation, identToken);
        }

        throw Logger.Fatal(new InvalidCallException(token.Location));
    }
}
