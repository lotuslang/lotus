public sealed class LeftParenParslet : IPrefixParslet<ValueNode>
{
    public ValueNode Parse(ExpressionParser parser, Token leftParenToken) {
        if (leftParenToken != "(")
            throw Logger.Fatal(new InvalidCallException(leftParenToken.Location));

        parser.Tokenizer.Reconsume();

        var valueTuple = parser.ConsumeTuple("(", ")");

        if (valueTuple.Count == 0) {
            Logger.Error(new UnexpectedTokenException(
                message: "Empty \"expression parentheses\" and tuples are not allowed. Did you forget a name or a value ?",
                token: parser.Tokenizer.Consume()
            ) { Position = new LocationRange(leftParenToken.Location, parser.Tokenizer.Current.Location) });

            return new ParenthesizedValueNode(ValueNode.NULL, leftParenToken, parser.Tokenizer.Current, isValid: false);
        }

        if (valueTuple.Count == 1) {
            return new ParenthesizedValueNode(
                valueTuple.Values[0],
                leftParenToken,
                valueTuple.ClosingToken,
                valueTuple.IsValid
            );
        }

        return valueTuple;
    }
}
