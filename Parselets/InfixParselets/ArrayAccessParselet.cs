using System.Linq;

public sealed class ArrayAccessParselet : IInfixParselet<OperationNode>
{
    public Precedence Precedence {
        get => Precedence.ArrayAccess;
    }

    public OperationNode Parse(ExpressionParser parser, Token openingBracket, ValueNode array) {

        if (openingBracket != "[")
            throw Logger.Fatal(new InvalidCallException(openingBracket.Location));

        var isValid = true;

        parser.Tokenizer.Reconsume();

        var indexes = parser.ConsumeTuple("[", "]");

        return new OperationNode(
            new OperatorToken(
                '[',
                Precedence.ArrayAccess,
                true,
                openingBracket.Location,
                openingBracket.IsValid,
                leading: openingBracket.LeadingTrivia,
                trailing: openingBracket.TrailingTrivia
            ),
            new[] { array }.Concat(indexes.Values).ToArray(), // FIXME: It hurts my eyes
            OperationType.ArrayAccess,
            isValid && indexes.IsValid,
            additionalTokens: indexes.ClosingToken
        );
    }
}