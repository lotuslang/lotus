public sealed class ArrayAccessParslet : IInfixParslet<OperationNode>
{
    public Precedence Precedence => Precedence.ArrayAccess;

    public static readonly ArrayAccessParslet Instance = new();

    public OperationNode Parse(ExpressionParser parser, Token openingBracket, ValueNode array) {

        if (openingBracket != "[")
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, openingBracket.Location));

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