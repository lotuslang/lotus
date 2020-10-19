using System.Linq;

public sealed class ArrayAccessParselet : IInfixParselet<OperationNode>
{
    public Precedence Precedence {
        get => Precedence.ArrayAccess;
    }

    public OperationNode Parse(Parser parser, Token openingBracket, ValueNode array) {

        if (openingBracket != "[")
            throw Logger.Fatal(new InvalidCallException(openingBracket.Location));

        var isValid = true;

        parser.Tokenizer.Reconsume();

        var indexes = parser.ConsumeCommaSeparatedValueList("[", "]", ref isValid, out Token closingBracket);

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
            new[]{array}.Concat(indexes).ToArray(), // FIXME: It hurts my eyes
            OperationType.ArrayAccess,
            isValid,
            additionalTokens: closingBracket
        );
    }
}