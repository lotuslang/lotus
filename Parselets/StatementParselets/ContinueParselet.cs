using System;

public class ContinueParselet : IStatementParselet<ContinueNode>
{
    public ContinueNode Parse(Parser parser, Token continueToken) {
        if (!(continueToken is ComplexToken continueKeyword && continueKeyword == "continue")) {
            throw Logger.Fatal(new InvalidCallException(continueToken.Location));
        }

        return new ContinueNode(continueKeyword);
    }
}