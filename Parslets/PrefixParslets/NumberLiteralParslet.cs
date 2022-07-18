public sealed class NumberLiteralParslet : IPrefixParslet<NumberNode>
{
    public static readonly NumberLiteralParslet Instance = new();

    public NumberNode Parse(ExpressionParser parser, Token token) {
        if (token is NumberToken numberToken) {
            return new NumberNode(numberToken);
        }

        throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, token.Location));
    }
}
