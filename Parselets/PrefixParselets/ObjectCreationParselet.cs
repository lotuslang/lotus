using System;
using System.Linq;


public class ObjectCreationParselet : IPrefixParselet
{
    public StatementNode Parse(Parser parser, Token newKeyword) {

        // if the token isn't the keyword "new", throw an exception
        if (newKeyword != "new") throw new UnexpectedTokenException(newKeyword, "in object initialization", "new");

        // basically, since a constructor invocation is basically just a function call preceded by the 'new' keyword
        // we can parse just eat the keyword and parse the rest as a function call, no need to write twice a code
        // that is so similar and essential
        var invoc = parser.ConsumeValue();

        if (invoc is FunctionCallNode call) {
            return new ObjectCreationNode(call, newKeyword as ComplexToken);
        }

        throw new Exception();
    }
}