using System;

public sealed class BoolLiteralParselet : IPrefixParselet<BoolNode>
{
    public BoolNode Parse(Parser _, Token boolToken) {
        if (boolToken is BoolToken boolean) {
            return new BoolNode(boolean.Value, boolean);
        }

        throw new ArgumentException("Token needs to be a bool.");
    }
}
