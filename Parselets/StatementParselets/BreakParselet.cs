using System;

public class BreakParselet : IStatementParselet<BreakNode>
{
    public BreakNode Parse(Parser parser, Token breakToken) {
        if (!(breakToken is ComplexToken breakKeyword && breakKeyword == "break")) {
            throw new Exception();
        }

        return new BreakNode(breakKeyword);
    }
}