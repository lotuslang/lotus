public sealed class ArrayAccessParslet : IInfixParslet<OperationNode>
{
    public Precedence Precedence => Precedence.ArrayAccess;

    public static readonly ArrayAccessParslet Instance = new();

    private static readonly ValueTupleParslet<ValueNode> _indexTupleParslet
        = new(static (parser) => parser.Consume()) {
            Start = "[",
            End = "]"
        };

    public OperationNode Parse(ExpressionParser parser, Token openingBracket, ValueNode array) {
        Debug.Assert(openingBracket == "[");

        var isValid = true;

        parser.Tokenizer.Reconsume();

        var indexTuple = _indexTupleParslet.Parse(parser);

        indexTuple = indexTuple with { Items = indexTuple.Items.Insert(0, array) };

        return new OperationNode(
            new OperatorToken(
                "[",
                Precedence.ArrayAccess,
                true,
                openingBracket.Location,
                openingBracket.IsValid
            ) { LeadingTrivia = openingBracket.LeadingTrivia, TrailingTrivia = openingBracket.TrailingTrivia } ,
            indexTuple.Items,
            OperationType.ArrayAccess,
            isValid && indexTuple.IsValid,
            additionalTokens: indexTuple.ClosingToken
        );
    }
}