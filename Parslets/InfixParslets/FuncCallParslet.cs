public sealed class FuncCallParslet : IInfixParslet<FunctionCallNode>
{
    public Precedence Precedence => Precedence.FuncCall;

    public static readonly FuncCallParslet Instance = new();

    public FunctionCallNode Parse(ExpressionParser parser, Token leftParen, ValueNode function) {
        if (leftParen != "(")
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, leftParen.Location));

        // reconsume the '(' for the ConsumeCommaSeparatedList() function
        parser.Tokenizer.Reconsume();

        var argsTuple = ConsumeFunctionArguments(parser);

        return new FunctionCallNode(
            argsTuple,
            function,
            argsTuple.IsValid
        );
    }

    public static TupleNode ConsumeFunctionArguments(ExpressionParser parser)
        => parser.ConsumeTuple("(", ")");
}
