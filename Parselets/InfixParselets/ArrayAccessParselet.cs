
public sealed class ArrayAccessParslet : IInfixParslet<OperationNode>
{
    public Precedence Precedence {
        get => Precedence.ArrayAccess;
    }

    private static ArrayAccessParslet _instance = new();
    public static ArrayAccessParslet Instance => _instance;

	private ArrayAccessParslet() : base() { }

    public OperationNode Parse(ExpressionParser parser, Token openingBracket, ValueNode array) {

        if (openingBracket != "[")
            throw Logger.Fatal(new InvalidCallException(openingBracket.Location));

        var isValid = true;

        parser.Tokenizer.Reconsume();

        var indexTuple = parser.ConsumeTuple("[", "]");

        indexTuple.Values.Insert(0, array);

        return new OperationNode(
            new OperatorToken(
                "[",
                Precedence.ArrayAccess,
                true,
                openingBracket.Location,
                openingBracket.IsValid
            ) { LeadingTrivia = openingBracket.LeadingTrivia, TrailingTrivia = openingBracket.TrailingTrivia } ,
            indexTuple.Values,
            OperationType.ArrayAccess,
            isValid && indexTuple.IsValid,
            additionalTokens: indexTuple.ClosingToken
        );
    }
}