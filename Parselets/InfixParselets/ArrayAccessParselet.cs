using System;
using System.Collections.Generic;

public class ArrayAccessParselet : IInfixParselet
{
    public Precedence Precedence {
        get => Precedence.ArrayAccess;
    }

    public StatementNode Parse(Parser parser, Token leftSquareBracket, StatementNode array) {
        if (!(array is ValueNode)) {
            throw new ArgumentException(nameof(array) + " needs to be, at least, an expression/value.");
        }

        var index = parser.ConsumeValue();

        if (parser.Tokenizer.Consume() != "]") {
            throw new UnexpectedTokenException(parser.Tokenizer.Current, "]");
        }

        return new OperationNode(
            new OperatorToken('[', Precedence.ArrayAccess, true, leftSquareBracket.Location),
            new ValueNode[] {
                array as ValueNode,
                index
            },
            "arrayAccess"
        );
    }
}