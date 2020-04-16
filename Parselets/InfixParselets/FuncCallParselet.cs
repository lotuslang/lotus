using System;
using System.Linq;

public class FuncCallParselet : IInfixParselet
{
    public Precedence Precedence {
        get => Precedence.FuncCall;
    }

    public StatementNode Parse(Parser parser, Token leftParen, StatementNode function) {

        if (!(function is ValueNode)) {
            throw new ArgumentException(nameof(function) + " needs to be, at least, a value.");
        }

        // reconsume the '(' for the ConsumeCommaSeparatedList() function
        parser.Tokenizer.Reconsume();

        var args = parser.ConsumeCommaSeparatedValueList("(", ")");

        return new FunctionCallNode(args, function as ValueNode, function.Token);
    }
}
