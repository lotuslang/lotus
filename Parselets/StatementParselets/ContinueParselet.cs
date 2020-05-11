using System;

public class ContinueParselet : IStatementParselet<ContinueNode>
{
    public ContinueNode Parse(Parser parser, Token continueToken) {
        if (!(continueToken is ComplexToken continueKeyword && continueKeyword == "continue")) {
            throw new Exception();
        }

        return new ContinueNode(continueKeyword);
    }
}