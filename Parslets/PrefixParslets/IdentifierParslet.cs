public sealed class IdentifierParslet : IPrefixParslet<IdentNode>
{
    public static readonly IdentifierParslet Instance = new();

    public IdentNode Parse(ExpressionParser parser, Token token) {
        var identToken = token as IdentToken;

        Debug.Assert(identToken is not null);

        return new IdentNode(identToken);
    }
}
