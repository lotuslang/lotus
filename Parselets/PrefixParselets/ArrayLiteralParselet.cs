using System;
using System.Linq;

public class ArrayLiteralParselet : IPrefixParselet
{
    public StatementNode Parse(Parser parser, Token token) {

        if (token != "[") throw new ArgumentException(nameof(token) + " needs to be a '[' (left square bracket).");

        parser.Tokenizer.Reconsume();

        var values = parser.ConsumeCommaSeparatedValueList("[", "]");

        return new ArrayLiteralNode(values, token);
    }
}
