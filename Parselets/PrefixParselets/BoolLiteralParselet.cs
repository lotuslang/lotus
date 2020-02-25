using System;

public class BoolLiteralParselet : IPrefixParselet
{
    public StatementNode Parse(Parser _, Token token) {
        if (token is BoolToken boolean) {
            return new BoolNode(boolean.Value, boolean);
        }

        throw new ArgumentException(nameof(token) + " needs to be a bool.");
    }
}
