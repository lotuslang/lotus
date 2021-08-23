using System.Linq;

public sealed class ArrayAccessParslet : IInfixParslet<OperationNode>
{
    public Precedence Precedence {
        get => Precedence.ArrayAccess;
    }

    public OperationNode Parse(ExpressionParser parser, Token openingBracket, ValueNode array) {

        if (openingBracket != "[")
            throw Logger.Fatal(new InvalidCallException(openingBracket.Location));

        var isValid = true;

        parser.Tokenizer.Reconsume();

        var indexTuple = parser.ConsumeTuple("[", "]");

        return new OperationNode(
            new OperatorToken(
                "[",
                Precedence.ArrayAccess,
                true,
                openingBracket.Location,
                openingBracket.IsValid
            ) { LeadingTrivia = openingBracket.LeadingTrivia, TrailingTrivia = openingBracket.TrailingTrivia } ,
            new[] { array }.Concat(indexTuple.Values).ToArray(), // FIXME: It hurts my eyes
            OperationType.ArrayAccess,
            isValid && indexTuple.IsValid,
            additionalTokens: indexTuple.ClosingToken
        );
    }
}