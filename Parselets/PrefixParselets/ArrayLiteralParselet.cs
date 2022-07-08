public sealed class ArrayLiteralParslet : IPrefixParslet<TupleNode>
{
    public static readonly ArrayLiteralParslet Instance = new();

    public TupleNode Parse(ExpressionParser parser, Token leftBracket) {
        if (leftBracket != "[")
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, leftBracket.Location));

        parser.Tokenizer.Reconsume();

        return parser.ConsumeTuple("[", "]");
    }
}
