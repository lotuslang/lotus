public sealed class TernaryOperatorParslet : IInfixParslet<OperationNode>
{
    public Precedence Precedence => Precedence.Ternary;

    private static TernaryOperatorParslet _instance = new();
    public static TernaryOperatorParslet Instance => _instance;

	private TernaryOperatorParslet() : base() { }

    public OperationNode Parse(ExpressionParser parser, Token questionMarkToken, ValueNode condition) {
        if (!(questionMarkToken is OperatorToken questionMarkOperator && questionMarkOperator == "?")) {
            throw Logger.Fatal(new InvalidCallException(questionMarkToken.Location));
        }

        var isValid = true;

        var firstValue = parser.Consume();

        var colon = parser.Tokenizer.Consume();

        if (colon != ":") {
            Logger.Error(new UnexpectedTokenException(
                token: colon,
                context: "in a ternary operator",
                expected: "the character ':'"
            ));

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        var secondValue = parser.Consume();

        return new OperationNode(questionMarkOperator, new[] { condition, firstValue, secondValue }, OperationType.Conditional, isValid, colon);
    }
}