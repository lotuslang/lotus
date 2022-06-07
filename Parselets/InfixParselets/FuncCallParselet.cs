public sealed class FuncCallParslet : IInfixParslet<FunctionCallNode>
{
    public Precedence Precedence {
        get => Precedence.FuncCall;
    }


    private static FuncCallParslet _instance = new();
    public static FuncCallParslet Instance => _instance;

	private FuncCallParslet() : base() { }

    public FunctionCallNode Parse(ExpressionParser parser, Token leftParen, ValueNode function) {
        if (leftParen != "(")
            throw Logger.Fatal(new InvalidCallException(leftParen.Location));

        var isValid = true;

        // reconsume the '(' for the ConsumeCommaSeparatedList() function
        parser.Tokenizer.Reconsume();

        var argsTuple = parser.ConsumeTuple("(", ")");

        return new FunctionCallNode(
            argsTuple,
            function,
            isValid && argsTuple.IsValid
        );
    }
}
