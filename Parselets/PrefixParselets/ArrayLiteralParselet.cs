public sealed class ArrayLiteralParselet : IPrefixParselet<TupleNode>
{
    public TupleNode Parse(ExpressionParser parser, Token leftBracket) {
        if (leftBracket != "[")
            throw Logger.Fatal(new InvalidCallException(leftBracket.Location));

        parser.Tokenizer.Reconsume();

        return parser.ConsumeTuple("[", "]");
    }
}
