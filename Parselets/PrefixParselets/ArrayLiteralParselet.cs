public sealed class ArrayLiteralParslet : IPrefixParslet<TupleNode>
{
    public TupleNode Parse(ExpressionParser parser, Token leftBracket) {
        if (leftBracket != "[")
            throw Logger.Fatal(new InvalidCallException(leftBracket.Location));

        parser.Tokenizer.Reconsume();

        return parser.ConsumeTuple("[", "]");
    }
}
