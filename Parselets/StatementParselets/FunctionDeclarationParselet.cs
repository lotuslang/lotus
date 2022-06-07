
public sealed class FunctionDeclarationParslet : IStatementParslet<FunctionDeclarationNode>
{

    private static FunctionDeclarationParslet _instance = new();
    public static FunctionDeclarationParslet Instance => _instance;

	private FunctionDeclarationParslet() : base() { }

    public FunctionDeclarationNode Parse(StatementParser parser, Token funcToken) {

        // if the token consumed was not "func", then throw an exception
        if (!(funcToken is Token funcKeyword && funcKeyword == "func"))
            throw Logger.Fatal(new InvalidCallException(funcToken.Location));

        var isValid = true;

        // consume the name of the function
        var funcNameToken = parser.Tokenizer.Consume();

        // if the token consumed was not an identifier, then throw an exception
        if (funcNameToken is not IdentToken funcName) {
            Logger.Error(new UnexpectedTokenException(
                token: funcNameToken,
                context: "in function declaration",
                expected: TokenKind.identifier
            ));

            isValid = false;

            if (funcNameToken == "(") parser.Tokenizer.Reconsume();

            funcName = new IdentToken(funcNameToken.Representation, funcNameToken.Location, false);
        }

        var openingParen = parser.Tokenizer.Consume();

        if (openingParen != "(") {
            Logger.Error(new UnexpectedTokenException(
                token: openingParen,
                context: "in function parameter list",
                expected: "("
            ));
        }

        // function parameter list format :
        // func Foo(someVar, string someStr, int [start, end]) : int
        // { /* some code */ }
        //

        var parameters = new List<FunctionParameter>();

        void addParameter(ValueNode type, ValueNode paramNameNode) {

            bool isValid = true;

            if (type != ValueNode.NULL && !Utilities.IsName(type)) {
                Logger.Error(new UnexpectedValueTypeException(
                    node: type,
                    context: "as a parameter type in a function's parameter list",
                    expected: "a type name (a qualified name)"
                ));

                isValid = false;
            }

            var defaultValue = ValueNode.NULL;

            var equalSign = Token.NULL;

            if (paramNameNode is OperationNode opNode && opNode.OperationType == OperationType.Assign) {
                paramNameNode = opNode.Operands[0];
                equalSign = opNode.Token;
                defaultValue = opNode.Operands[1];
            }

            if (paramNameNode is not IdentNode paramName) {
                if (!paramNameNode.IsValid) { // && Logger.Exceptions.Peek().Item1 is UnexpectedTokenException) {
                    Logger.Exceptions.Pop();
                }

                Logger.Error(new UnexpectedValueTypeException(
                    node: paramNameNode,
                    context: "as a parameter name in a function's parameter list",
                    expected: "a simple name (an identifier)"
                ));

                isValid = false;

                paramName = new IdentNode(
                    new IdentToken(paramNameNode.Token.Representation, paramNameNode.Token.Location, false),
                    false
                );
            }

            parameters!.Add(new FunctionParameter(type, paramName, defaultValue, equalSign, isValid));
        }

        while (parser.Tokenizer.Peek() != ")") {

            /*
            * Ok, this needs a bit of explanation. So, I wanted to implement a syntactic shorthand I'd like to see
            * in csharp (and other languages) that allows users to define the type of multiple parameters at the
            * same time if they have the same type. It can be really long and frustrating to have things like :
            *
            * ```
            * int DoThing(int seed, int cookie, int hash, int secret, int key) {
            *     return (new Random(seed).Next() * cookie ^ hash % key) & secret;
            * }
            * ```
            *
            * Therefore, I thought of a shorthand such as : `int DoThing(int [seed, cookie, hash, secret, key])`
            * As you probably noticed, it is really similar to an array init/literal (which is honestly a good
            * thing in my opinion). So I thought I would just parse the type, and then, if the next node wasn't an
            * IdentNode or a comma, I would just use the ArrayLiteral parslet and register each parameter with the type.
            *
            * Yeah, that's not what happened.
            *
            * What I didn't notice is that int [a, b, c] has actually kind of the same syntax as array indexing;
            * you just have to remove the space and it becomes quite apparent. I had to make array accesses able
            * to handle commas, but I'm not happy about it because it made more sense (to me) and was cleaner to
            * parse the type first and then an array of names. But alas, I have to do this for now, and I don't
            * think I could easily fix this without re-writing a special parsing algorithm in this method just
            * for member-access/qualified-names (i.e. the dot operator).
            *
            * Future blokyk here : This is a nightmare to understand and refactor. Thanks.
            *
            * Future future blokyk : I hate all of you. I have to modify this for the printers and this is just
            * disgusting to look at. Fuck all of you.
            */

            var typeOrName = parser.ExpressionParser.Consume();

            // i don't wanna talk about it any more than i already did

            if (typeOrName is OperationNode typeNameArray) {
                // if it's a list of parameters with the same type

                // if this isn't an array-access-like thing
                if (typeNameArray.OperationType != OperationType.ArrayAccess) {
                    Logger.Error(new UnexpectedValueTypeException(
                        node: typeNameArray,
                        context: "as a typed parameter-group in a function's parameter list",
                        expected: "a type name followed by an array-like syntax (e.g. int [param1, param2])"
                    ));

                    isValid = false;

                    // FIXME: Maybe we could try to consume until there's either a comma or a parenthesis,
                    // but if there isn't any (i.e. the function is malformed later on), we would have a problem :/

                    goto LOOP_END_CHECK;
                }

                var paramType = typeNameArray.Operands.FirstOrDefault();

                // if there's only one argument (illegal)
                if (typeNameArray.Operands.Count < 2) {
                    Logger.Error(new LotusException(
                        message: "A typed parameter-group should declare at least two parameters, which is not the case here.",
                        range: (paramType is not null && paramType != ValueNode.NULL ? paramType : typeOrName).Token.Location
                    ));

                    isValid = false;
                }

                // iterate over every parameter name and add them to the list
                // we skip one cause the first operand of an array access is the type name
                foreach (var paramNameNode in typeNameArray.Operands.Skip(1)) {
                    addParameter(paramType ?? ValueNode.NULL, paramNameNode);
                }
            } else if (parser.Tokenizer.Peek().Kind == TokenKind.identifier) {
                // if there's still an identifier after typeOrName,
                // it's a typed parameter (and the name is the token we peeked at)

                addParameter(typeOrName, parser.ExpressionParser.Consume());
            } else {
                // otherwise, that means we have a parameter without type info
                addParameter(ValueNode.NULL, typeOrName);
            }

        LOOP_END_CHECK: // FIXME: i know, this is bad practice, and i'm fully open to alternatives
            if (parser.Tokenizer.Consume() != ",") {
                if (parser.Tokenizer.Current == ")") {
                    parser.Tokenizer.Reconsume();

                    break;
                }

                Logger.Error(new UnexpectedTokenException(
                    token: parser.Tokenizer.Current,
                    context: "after a parameter in a function's parameter list",
                    expected: "a comma or a parenthesis"
                ));

                isValid = false;

                parser.Tokenizer.Reconsume();

                continue;
            }

            if (parser.Tokenizer.Peek() == ")") { // cause that would mean it is a comma followed directly by a parenthesis
                Logger.Error(new UnexpectedTokenException(
                    token: parser.Tokenizer.Peek(),
                    context: "after a comma in a function's parameter list",
                    expected: "a parameter name or type. Did you forget a parameter ?"
                ) { Position = new LocationRange(parser.Tokenizer.Current.Location, parser.Tokenizer.Peek().Location) });

                isValid = false;
            }
        }

        var closingParen = parser.Tokenizer.Consume();

        // this means we probably broke because of an EOF
        if (closingParen != ")") {
            Logger.Error(new UnexpectedEOFException(
                context: "in a function's parameter list",
                expected: "a closing parenthesis ')'",
                range: parser.Tokenizer.Position
            ));

            isValid = false;
        }

        var returnType = ValueNode.NULL;

        var colonToken = Token.NULL;

        if (parser.Tokenizer.Peek() == ":") {
            colonToken = parser.Tokenizer.Consume(); // consume the colon

            returnType = parser.ExpressionParser.Consume();

            if (!Utilities.IsName(returnType)) {
                Logger.Error(new UnexpectedValueTypeException(
                    node: returnType,
                    context: "as a return type in a function declaration",
                    expected: "a type name"
                ));

                isValid = false;
            }
        }

        // consume a simple block
        var block = parser.ConsumeSimpleBlock(areOneLinersAllowed: false);

        // return a new FunctionDeclarationNode with 'block' as the value, 'parameters' as the list of params, and funcName as the name
        return new FunctionDeclarationNode(
            block,
            parameters,
            returnType,
            funcName,
            funcKeyword,
            openingParen,
            closingParen,
            colonToken,
            isValid
        );
    }
}
