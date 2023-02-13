namespace Lotus.Syntax;

public sealed class ObjectCreationParslet : IPrefixParslet<ObjectCreationNode>
{
    public static readonly ObjectCreationParslet Instance = new();

    public ObjectCreationNode Parse(ExpressionParser parser, Token newToken) {
        Debug.Assert(newToken == "new");

        var isValid = true;

        // basically, since a constructor invocation is basically just a function call preceded by the 'new' keyword
        // we can just eat the keyword and parse the rest as a function call, no need to write twice a code
        // that is so similar and essential

        // cases to check for acceptance :
        //      - new string() + world
        //      - new string()[0]
        //      - new string()[1]
        //      - new string.world()
        //      - new string.hello()[1].hi
        //
        // cases to check for rejection ;
        //      - new (string.hello)()
        //      - new ++string()
        //      - new string[0]()
        //      - new string[0].hello()
        //      - new new string()()
        //      - new (new string())()
        //
        // NB : Better we parse incorrect stuff than reject valid code.
        // We can validate, but it's not our main goal. Here, we need to ensure
        // that errors don't jeopardize the stability of the parser or of later
        // phases

        var type = parser.Consume(Precedence.FuncCall);

        if (type is not NameNode typeName) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = type,
                In = "an object instantiation",
                Expected = "a type name."
            });

            isValid = false;

            if (type.IsValid)
                parser.Tokenizer.Reconsume();

            typeName = new IdentNode(
                new IdentToken(
                    type.Token.Representation,
                    type.Location
                ) { IsValid = false }
            );
        }

        // calling FuncCall.Parse is really dangerous, since we could get an InvalidCallException
        // if typeName isn't correctly parsed/lexed
        var args = FuncCallParslet.ConsumeFunctionArguments(parser);

        var call = new FunctionCallNode(args, typeName) { IsValid = args.IsValid && type.IsValid };

        return new ObjectCreationNode(call, newToken) { IsValid = isValid };
    }
}