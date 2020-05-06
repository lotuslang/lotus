using System;

public sealed class NumberLiteralParselet : IPrefixParselet<NumberNode>
{
    public NumberNode Parse(Parser _, Token token) {
        if (token is NumberToken number) {
            return new NumberNode(number);
        }

        throw new ArgumentException("Token needs to be a number.");
    }
}
