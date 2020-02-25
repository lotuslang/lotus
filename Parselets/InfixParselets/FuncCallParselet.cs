using System;

public class FuncCallParselet : IInfixParselet
{
    public Precedence Precedence {
        get => Precedence.FuncCall;
    }

    public StatementNode Parse(Parser parser, Token leftParen, StatementNode function) {

        if (!(function is ValueNode)) {
            throw new ArgumentException(nameof(function) + " needs to be, at least, a value.");
        }

        parser.Tokenizer.Reconsume();

        var args = parser.ConsumeCommaSeparatedList("(", ")");

        return new FunctionCallNode(args, function as ValueNode, leftParen);
    }
}
