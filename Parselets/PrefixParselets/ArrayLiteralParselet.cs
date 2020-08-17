using System;
using System.Linq;

public sealed class ArrayLiteralParselet : IPrefixParselet<ArrayLiteralNode>
{
    public ArrayLiteralNode Parse(Parser parser, Token leftSquareBracket) {

        if (leftSquareBracket != "[") {
            throw Logger.Fatal(new InvalidCallException(leftSquareBracket.Location));
        }

        var isValid = true;

        parser.Tokenizer.Reconsume(); // reconsume the square bracket '['

        var values = parser.ConsumeCommaSeparatedValueList("[", "]", ref isValid);

        return new ArrayLiteralNode(values, leftSquareBracket, isValid);
    }
}
