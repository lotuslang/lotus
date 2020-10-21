public sealed class ObjectCreationParselet : IPrefixParselet<ObjectCreationNode>
{
    public ObjectCreationNode Parse(Parser parser, Token newToken) {

        // if the token isn't the keyword "new", throw an exception
        if (!(newToken is ComplexToken newKeyword && newKeyword == "new"))
            throw Logger.Fatal(new InvalidCallException(newToken.Location));

        var isValid = true;

        // basically, since a constructor invocation is basically just a function call preceded by the 'new' keyword
        // we can just eat the keyword and parse the rest as a function call, no need to write twice a code
        // that is so similar and essential
        var invoc = parser.ConsumeValue();

        if (!(invoc is FunctionCallNode call)) {
            Logger.Error(new UnexpectedValueTypeException(
                node: invoc,
                context: "in object instantiation",
                expected: typeof(FunctionCallNode)
            ));

            isValid = false;

            call = new FunctionCallNode(new TupleNode(new ValueNode[0], Token.NULL, Token.NULL), invoc, invoc.Token, isValid: false);

            parser.Tokenizer.Reconsume();
        }

        return new ObjectCreationNode(call, newKeyword, isValid);
    }
}