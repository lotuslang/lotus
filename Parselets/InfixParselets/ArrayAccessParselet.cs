using System;
using System.Collections.Generic;

public sealed class ArrayAccessParselet : IInfixParselet<OperationNode>
{
    public Precedence Precedence {
        get => Precedence.ArrayAccess;
    }

    public OperationNode Parse(Parser parser, Token leftSquareBracketToken, ValueNode array) {

        if (leftSquareBracketToken != "[")
            throw new UnexpectedTokenException(leftSquareBracketToken, "in array index access", "[");

        var index = parser.ConsumeValue();

        if (parser.Tokenizer.Consume() != "]") {
            throw new UnexpectedTokenException(parser.Tokenizer.Current, "in array index access", "]");
        }

        return new OperationNode(
            new OperatorToken('[', Precedence.ArrayAccess, true, leftSquareBracketToken.Location),
            new ValueNode[] {
                array,
                index
            },
            OperationType.ArrayAccess
        );
    }
}