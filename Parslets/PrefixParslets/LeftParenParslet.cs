public sealed class LeftParenParslet : IPrefixParslet<ValueNode>
{
    public static readonly LeftParenParslet Instance = new();

    public ValueNode Parse(ExpressionParser parser, Token leftParenToken) {
        Debug.Assert(leftParenToken == "(");

        parser.Tokenizer.Reconsume();

        var valueTuple = new TupleNode(parser.ConsumeTuple());

        if (valueTuple.Count == 0) {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Message = "Empty \"precedence parentheses\" and tuples are not allowed. Did you forget a name or a value ?",
                Value = parser.Tokenizer.Consume(),
                Location = new LocationRange(leftParenToken.Location, parser.Tokenizer.Current.Location)
            });

            return new ParenthesizedValueNode(ValueNode.NULL, leftParenToken, parser.Tokenizer.Current, isValid: false);
        }

        return valueTuple.Count == 1 ? valueTuple.AsParenthesized() : valueTuple;
    }
}
