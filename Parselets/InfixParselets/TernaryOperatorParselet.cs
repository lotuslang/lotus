public sealed class TernaryOperatorParselet : IInfixParselet<OperationNode>
{
    public Precedence Precedence => Precedence.Ternary;

    public OperationNode Parse(Parser parser, Token questionMarkToken, ValueNode condition) {
        if (!(questionMarkToken is OperatorToken questionMarkOperator && questionMarkOperator == "?")) {
            throw Logger.Fatal(new InvalidCallException(questionMarkToken.Location));
        }

        var isValid = true;

        var firstValue = parser.ConsumeValue();

        if (parser.Tokenizer.Consume() != ":") {
            Logger.Error(new UnexpectedTokenException(
                token: parser.Tokenizer.Current,
                context: "in a ternary operator",
                expected: "the character ':'"
            ));

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        var secondValue = parser.ConsumeValue();

        return new OperationNode(questionMarkOperator, new[] {condition, firstValue, secondValue}, OperationType.Conditional, isValid);
    }
}