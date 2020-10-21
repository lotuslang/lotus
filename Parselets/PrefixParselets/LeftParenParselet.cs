public sealed class LeftParenParselet : IPrefixParselet<ValueNode>
{
    public ValueNode Parse(Parser parser, Token leftParenToken) {
        if (leftParenToken != "(")
            throw Logger.Fatal(new InvalidCallException(leftParenToken.Location));
        if (parser.Tokenizer.Peek() == ")") {
            Logger.Error(new UnexpectedTokenException(
                message: "Empty \"expression parentheses\" are not allowed. Did you forget a name or a value ?",
                token: parser.Tokenizer.Consume()
            ) { Position = new LocationRange(leftParenToken.Location, parser.Tokenizer.Current.Location)});

            return new ParenthesizedValueNode(leftParenToken, parser.Tokenizer.Current, ValueNode.NULL, isValid: false);
        }

        var value = parser.ConsumeValue();

        var rightParenToken = parser.Tokenizer.Consume();

        // if the next token isn't a right/closing parenthesis, throw an error
        if (rightParenToken != ")") {
            Logger.Error(new UnexpectedTokenException(
                token: parser.Tokenizer.Current,
                context: "after a parenthesized expression",
                expected: ")"
            ));

            value.IsValid = false;

            parser.Tokenizer.Reconsume(); // TODO: could we try to parse this as a list of values (still throw tho)
        }

        return new ParenthesizedValueNode(leftParenToken, rightParenToken, value);
    }
}
