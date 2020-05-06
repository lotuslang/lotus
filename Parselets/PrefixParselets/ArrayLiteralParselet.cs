using System;
using System.Linq;

public sealed class ArrayLiteralParselet : IPrefixParselet<ArrayLiteralNode>
{
    public ArrayLiteralNode Parse(Parser parser, Token leftSquareBracket) {

        if (leftSquareBracket != "[")
            throw new ArgumentException("Token needs to be a '[' (left square bracket).");

        parser.Tokenizer.Reconsume(); // reconsume the square bracket '['

        var values = parser.ConsumeCommaSeparatedValueList("[", "]");

        return new ArrayLiteralNode(values, leftSquareBracket);
    }
}
