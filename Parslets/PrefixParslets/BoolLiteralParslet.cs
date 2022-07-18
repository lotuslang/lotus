public sealed class BoolLiteralParslet : IPrefixParslet<BoolNode>
{
    public static readonly BoolLiteralParslet Instance = new();

    public BoolNode Parse(ExpressionParser parser, Token token) {
        if (token is not BoolToken boolToken) {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, token.Location));
        }

        return new BoolNode(boolToken);
    }
}
