using System;

public class BreakParselet : IStatementParselet<BreakNode>
{
    public BreakNode Parse(Parser parser, Token breakToken) {
        if (!(breakToken is ComplexToken breakKeyword && breakKeyword == "break")) {
            throw Logger.Fatal(new InvalidCallException(breakToken.Location));
        }

        return new BreakNode(breakKeyword);
    }
}