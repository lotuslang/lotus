public sealed class FuncCallParslet : IInfixParslet<FunctionCallNode>
{
    public Precedence Precedence {
        get => Precedence.FuncCall;
    }

    public FunctionCallNode Parse(ExpressionParser parser, Token leftParen, ValueNode function) {
        if (leftParen != "(")
            throw Logger.Fatal(new InvalidCallException(leftParen.Location));

        var isValid = true;

        /*if (!Utilities.IsName(function)) {
            Logger.Error(new UnexpectedValueTypeException(
                node: function,
                context: "as a function name",
                expected: "a name (simple (`someFunc`) or a qualified one (`object.toString`)"
            ));

            isValid = false;
        }*/

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
