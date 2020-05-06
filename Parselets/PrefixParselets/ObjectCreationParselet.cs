using System;
using System.Linq;

public sealed class ObjectCreationParselet : IPrefixParselet<ObjectCreationNode>
{
    public ObjectCreationNode Parse(Parser parser, Token newKeyword) {

        // if the token isn't the keyword "new", throw an exception
        if (!(newKeyword is ComplexToken newToken && newKeyword == "new")) throw new UnexpectedTokenException(newKeyword, "in object initialization", "new");

        // basically, since a constructor invocation is basically just a function call preceded by the 'new' keyword
        // we can parse just eat the keyword and parse the rest as a function call, no need to write twice a code
        // that is so similar and essential
        var invoc = parser.ConsumeValue();

        if (!(invoc is FunctionCallNode call)) {
            throw new UnexpectedValueType(invoc, "in object instantiation", typeof(FunctionCallNode));
        }

        return new ObjectCreationNode(call, newToken);
    }
}