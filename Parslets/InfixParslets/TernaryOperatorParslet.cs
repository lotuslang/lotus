public sealed class TernaryOperatorParslet : IInfixParslet<OperationNode>
{
    public Precedence Precedence => Precedence.Ternary;

    public static readonly TernaryOperatorParslet Instance = new();

    public OperationNode Parse(ExpressionParser parser, Token questionMarkToken, ValueNode condition) {
        var questionMarkOperator = questionMarkToken as OperatorToken;

        Debug.Assert(questionMarkOperator is not null);
        Debug.Assert(questionMarkOperator.Representation == "?");

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

        return new OperationNode(questionMarkOperator, ImmutableArray.Create(condition, firstValue, secondValue), OperationType.Conditional, ImmutableArray.Create(colon)) { IsValid = isValid };
    }
}