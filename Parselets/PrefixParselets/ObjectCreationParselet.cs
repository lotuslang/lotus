
public sealed class ObjectCreationParslet : IPrefixParslet<ObjectCreationNode>
{
    public ObjectCreationNode Parse(ExpressionParser parser, Token newToken) {

        // if the token isn't the keyword "new", throw an exception
        if (!(newToken is Token newKeyword && newKeyword == "new"))
            throw Logger.Fatal(new InvalidCallException(newToken.Location));

        var isValid = true;

        // basically, since a constructor invocation is basically just a function call preceded by the 'new' keyword
        // we can just eat the keyword and parse the rest as a function call, no need to write twice a code
        // that is so similar and essential
        var invoc = parser.Consume();

        if (invoc is not FunctionCallNode call) {
            Logger.Error(new UnexpectedValueTypeException(
                node: invoc,
                context: "in object instantiation",
                expected: typeof(FunctionCallNode)
            ));

            isValid = false;

            call = new FunctionCallNode(new TupleNode(Array.Empty<ValueNode>(), Token.NULL, Token.NULL), invoc, false);

            parser.Tokenizer.Reconsume();
        }

        return new ObjectCreationNode(call, newKeyword, isValid);
    }
}