public sealed class ArrayLiteralParslet : IPrefixParslet<TupleNode>
{

    private static ArrayLiteralParslet _instance = new();
    public static ArrayLiteralParslet Instance => _instance;

	private ArrayLiteralParslet() : base() { }

    public TupleNode Parse(ExpressionParser parser, Token leftBracket) {
        if (leftBracket != "[")
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, leftBracket.Location));

        parser.Tokenizer.Reconsume();

        return parser.ConsumeTuple("[", "]");
    }
}
