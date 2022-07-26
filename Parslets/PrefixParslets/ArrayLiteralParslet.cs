public sealed class ArrayLiteralParslet : IPrefixParslet<TupleNode>
{
    public static readonly ArrayLiteralParslet Instance = new();

    public TupleNode Parse(ExpressionParser parser, Token leftBracket) {
        Debug.Assert(leftBracket == "[");

        parser.Tokenizer.Reconsume();

        return new TupleNode(parser.ConsumeTuple("[", "]"));
    }
}
