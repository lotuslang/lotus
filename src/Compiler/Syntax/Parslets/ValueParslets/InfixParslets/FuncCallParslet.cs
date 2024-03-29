namespace Lotus.Syntax;

public sealed class FuncCallParslet : IInfixParslet<FunctionCallNode>
{
    public Precedence Precedence => Precedence.FuncCall;

    public static readonly FuncCallParslet Instance = new();

    public FunctionCallNode Parse(Parser parser, Token leftParen, ValueNode function) {
        Debug.Assert(leftParen == "(");

        // reconsume the '(' for the ConsumeCommaSeparatedList() function
        parser.Tokenizer.Reconsume();

        var argsTuple = ConsumeFunctionArguments(parser);

        return new FunctionCallNode(
            argsTuple,
            function
        ) { IsValid = argsTuple.IsValid };
    }

    public static Tuple<ValueNode> ConsumeFunctionArguments(Parser parser)
        => parser.ConsumeTuple();
}