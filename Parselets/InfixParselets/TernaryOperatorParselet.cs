public sealed class TernaryOperatorParslet : IInfixParslet<OperationNode>
{
    public Precedence Precedence => Precedence.Ternary;

    private static TernaryOperatorParslet _instance = new();
    public static TernaryOperatorParslet Instance => _instance;

	private TernaryOperatorParslet() : base() { }

    public OperationNode Parse(ExpressionParser parser, Token questionMarkToken, ValueNode condition) {
        if (!(questionMarkToken is OperatorToken questionMarkOperator && questionMarkOperator == "?")) {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, questionMarkToken.Location));
        }

        var isValid = true;

        var firstValue = parser.Consume();

        var colon = parser.Tokenizer.Consume();

        if (colon != ":") {
            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        var secondValue = parser.Consume();

        if (!isValid) { // move it after consuming the second value to give better suggestions
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = secondValue,
                In = "a ternary operator",
                Expected = "a colon ':'",
                ExtraNotes = "To separate the two branches, you have to use a colon ':'\n\t"
                            +ASTHelper.PrintValue(condition)
                            +ASTHelper.PrintToken(questionMarkToken)
                            +ASTHelper.PrintValue(firstValue)
                            +": " // no space because previous trivia token probably takes care of it
                            +ASTHelper.PrintValue(secondValue)
            });
        }

        return new OperationNode(questionMarkOperator, new[] { condition, firstValue, secondValue }, OperationType.Conditional, isValid, colon);
    }
}