
public sealed class ObjectCreationParslet : IPrefixParslet<ObjectCreationNode>
{

    private static ObjectCreationParslet _instance = new();
    public static ObjectCreationParslet Instance => _instance;

	private ObjectCreationParslet() : base() { }

    public ObjectCreationNode Parse(ExpressionParser parser, Token newToken) {

        // if the token isn't the keyword "new", throw an exception
        if (newToken is not Token newKeyword || newKeyword != "new")
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, newToken.Location));

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
        //      - new (string.hello)()       <--- INVALID but accept anyway...
        //
        // cases to check for rejection ;
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

        var typeName = parser.Consume(Precedence.FuncCall);

        if (!Utilities.IsName(typeName)) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = typeName,
                In = "an object instantiation",
                Expected = "a type name."
            });

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        // calling FuncCall.Parse is really dangerous, since we could get an InvalidCallException
        // if typeName isn't correctly parsed/lexed
        var args = FuncCallParslet.ConsumeFunctionArguments(parser);

        var call = new FunctionCallNode(args, typeName, args.IsValid && typeName.IsValid);

        return new ObjectCreationNode(call, newKeyword, isValid);
    }
}