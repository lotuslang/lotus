public sealed class IdentifierParslet : IPrefixParslet<IdentNode>
{
    public static readonly IdentifierParslet Instance = new();

    public IdentNode Parse(ExpressionParser parser, Token token) {
        if (token is not IdentToken identToken) {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, token.Location));
        }

        return new IdentNode(identToken);
    }
}
