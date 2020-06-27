using System;
using System.Linq;

public sealed class FuncCallParselet : IInfixParselet<FunctionCallNode>
{
    public Precedence Precedence {
        get => Precedence.FuncCall;
    }

    public FunctionCallNode Parse(Parser parser, Token leftParenToken, ValueNode function) {

        if (leftParenToken != "(") throw new UnexpectedTokenException(leftParenToken, "in function call", "(");

        if (function is NumberNode) throw new Exception("wtf");

        // reconsume the '(' for the ConsumeCommaSeparatedList() function
        parser.Tokenizer.Reconsume();

        var args = parser.ConsumeCommaSeparatedValueList("(", ")");

        return new FunctionCallNode(args, function, function.Token);
    }
}
