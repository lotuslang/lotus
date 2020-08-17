using System;
using System.Linq;
using System.Collections.Generic;

public sealed class ArrayAccessParselet : IInfixParselet<OperationNode>
{
    public Precedence Precedence {
        get => Precedence.ArrayAccess;
    }

    public OperationNode Parse(Parser parser, Token leftSquareBracketToken, ValueNode array) {

        if (leftSquareBracketToken != "[")
            throw Logger.Fatal(new InvalidCallException(leftSquareBracketToken.Location));

        var isValid = true;

        parser.Tokenizer.Reconsume();

        var indexes = parser.ConsumeCommaSeparatedValueList("[", "]", ref isValid);

        return new OperationNode(
            new OperatorToken('[', Precedence.ArrayAccess, true, leftSquareBracketToken.Location),
            new[]{array}.Concat(indexes).ToArray(), // FIXME: It hurts my eyes
            OperationType.ArrayAccess,
            isValid
        );
    }
}