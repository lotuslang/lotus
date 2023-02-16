namespace Lotus.Syntax;

public sealed class TernaryOperatorParslet : IInfixParslet<OperationNode>
{
    public Precedence Precedence => Precedence.Ternary;

    public static readonly TernaryOperatorParslet Instance = new();

    public OperationNode Parse(Parser parser, Token questionMarkToken, ValueNode condition) {
        var questionMarkOperator = questionMarkToken as OperatorToken;

        Debug.Assert(questionMarkOperator is { Representation: "?" });

        var isValid = true;

        var firstValue = parser.ConsumeValue();

        var colon = parser.Tokenizer.Consume();

        if (colon != ":") {
            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        var secondValue = parser.ConsumeValue();

        if (!isValid) { // move it after consuming the second value to give better suggestions
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = secondValue,
                In = "a ternary operator",
                Expected = "a colon ':'",
                ExtraNotes = "To separate the two branches, you have to use a colon ':'\n\t"
                            +ASTUtils.PrintValue(condition)
                            +ASTUtils.PrintToken(questionMarkToken)
                            +ASTUtils.PrintValue(firstValue)
                            +": " // no space because previous trivia token probably takes care of it
                            +ASTUtils.PrintValue(secondValue)
            });
        }

        return new OperationNode(questionMarkOperator, ImmutableArray.Create(condition, firstValue, secondValue), OperationType.Conditional, ImmutableArray.Create(colon)) { IsValid = isValid };
    }
}