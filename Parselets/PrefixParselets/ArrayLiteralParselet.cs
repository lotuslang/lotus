public sealed class ArrayLiteralParselet : IPrefixParselet<ArrayLiteralNode>
{
    public ArrayLiteralNode Parse(Parser parser, Token leftBracket) {

        if (leftBracket != "[") {
            throw Logger.Fatal(new InvalidCallException(leftBracket.Location));
        }

        var isValid = true;

        parser.Tokenizer.Reconsume(); // reconsume the square bracket '['

        var values = parser.ConsumeCommaSeparatedValueList("[", "]", ref isValid, out Token rightBracket);

        return new ArrayLiteralNode(values, leftBracket, rightBracket, isValid);
    }
}
