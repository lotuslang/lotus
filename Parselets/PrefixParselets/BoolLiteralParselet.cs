using System;

public sealed class BoolLiteralParselet : IPrefixParselet<BoolNode>
{
    public BoolNode Parse(Parser _, Token token) {
        if (token is BoolToken boolToken) {
            return new BoolNode(boolToken.Value, boolToken);
        }

        throw Logger.Fatal(new InvalidCallException(token.Location));
    }
}
