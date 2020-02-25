using System;

public class NumberLiteralParselet : IPrefixParselet
{
    public StatementNode Parse(Parser _, Token token) {
        if (token is NumberToken number) {
            return new NumberNode(number);
        }

        throw new ArgumentException(nameof(token) + " needs to be a number.");
    }
}
