public sealed class IdentifierParslet : IPrefixParslet<IdentNode>
{
    public IdentNode Parse(ExpressionParser parser, Token token) {
        if (token is not IdentToken identToken) {
            throw Logger.Fatal(new InvalidCallException(token.Location));
        }

        return new IdentNode(token.Representation, identToken);
    }
}
