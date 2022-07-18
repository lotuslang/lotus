public sealed class FunctionDeclarationParslet : IStatementParslet<FunctionDeclarationNode>
{
    public static readonly FunctionDeclarationParslet Instance = new();

    public FunctionDeclarationNode Parse(StatementParser parser, Token funcToken) {

        // if the token consumed was not "func", then throw an exception
        if (funcToken is not Token funcKeyword || funcKeyword != "func")
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, funcToken.Location));

        var isValid = true;

        // consume the name of the function
        var funcNameToken = parser.Tokenizer.Consume();

        // if the token consumed was not an identifier, then throw an exception
        if (funcNameToken is not IdentToken funcName) {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = funcNameToken,
                In = "a function declaration",
                Expected = "an identifier"
            });

            isValid = false;

            if (funcNameToken == "(") parser.Tokenizer.Reconsume();

            funcName = new IdentToken(funcNameToken.Representation, funcNameToken.Location, false);
        }

        var openingParen = parser.Tokenizer.Consume();

        if (openingParen != "(") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = openingParen,
                In = "a function parameter list",
                Expected = "("
            });
        }

        // function parameter list format :
        // func Foo(someVar, string someStr, int [start, end]) : int
        // { /* some code */ }
        //

        var parameters = new List<FunctionParameter>();

        void addParameter(ValueNode type, ValueNode paramNameNode) {

            bool isValid = true;

            if (type is not NameNode typeName) {
                Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                    Value = type,
                    As = "a param type in a function's parameter list",
                    Expected = "a type name"
                });

                isValid = false;

                typeName = new IdentNode(
                    new IdentToken(
                        type.Token,
                        type.Location,
                        false
                    ),
                    false
                );
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
                    Logger.errorStack.Pop();
                }

                Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                    Value = paramNameNode,
                    As = "a param name in a function's parameter list",
                    Expected = "an identifier"
                });

                isValid = false;

                paramName = new IdentNode(
                    new IdentToken(paramNameNode.Token.Representation, paramNameNode.Token.Location, false),
                    false
                );
            }

            parameters!.Add(new FunctionParameter(typeName, paramName, defaultValue, equalSign, isValid));
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

            var typeName = parser.ExpressionParser.Consume();

            // i don't wanna talk about it any more than i already did

            if (typeName is OperationNode typeNameArray) {
                // if it's a list of parameters with the same type

                // if this isn't an array-access-like thing
                if (typeNameArray.OperationType != OperationType.ArrayAccess) {
                    Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                        Value = typeNameArray,
                        As = "a typed parameter-group in a function's parameter list",
                        Expected = "a type name followed by an array-like syntax (e.g. int [param1, param2])"
                    });

                    isValid = false;

                    // FIXME: Maybe we could try to consume until there's either a comma or a parenthesis,
                    // but if there isn't any (i.e. the function is malformed later on), we would have a problem :/

                    goto LOOP_END_CHECK;
                }

                var paramType = typeNameArray.Operands.FirstOrDefault(
                    NameNode.NULL with {
                        Location = typeNameArray.Token.Location
                    }
                );

                // if there's less than 2 names (illegal)
                if (typeNameArray.Operands.Count < 2) { // FIXME: We should really make an error type for wrong-numbered things

                    var errorLoc = paramType.Location;

                    if (typeNameArray.Operands.Count == 0) {
                        errorLoc =
                            new LocationRange(
                                typeNameArray.AdditionalTokens[0].Location,
                                typeNameArray.AdditionalTokens[1].Location
                            );
                    }

                    Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                        Value = typeNameArray.Operands[0],
                        Location = errorLoc,
                        Message = "A parameter-group should declare at least two parameters, instead of just " + typeNameArray.Operands.Count,
                    });

                    isValid = false;
                }

                // iterate over every parameter name and add them to the list
                // we skip one cause the first operand of an array access is the type name
                foreach (var paramNameNode in typeNameArray.Operands.Skip(1)) {
                    addParameter(paramType, paramNameNode);
                }
            } else if (parser.Tokenizer.Peek() != ",") {
                // if there's no comma after the type, then there's probably a param's name

                addParameter(typeName, parser.ExpressionParser.Consume());
            } else {
                // otherwise, that means we have a parameter without type info
                //! in that case, typeName is probably a parameter name

                // add it anyway for consistency and in case the param name
                // isn't even a name, because addParameter handles that for us
                addParameter(NameNode.NULL, typeName);

                // if the param name wasn't valid, no need to emit a second misleading
                // error
                if (!parameters.Last().IsValid) {
                    isValid = false;
                    goto LOOP_END_CHECK;
                }

                var notes = "Did you forget to specify a type for this parameter ? "
                          + "For example, you could write :\n\t"
                          + "func "
                          + ASTHelper.PrintToken(funcName)
                          + ASTHelper.PrintToken(openingParen);

                if (parameters.Count != 0) {
                    notes += "..., ";
                }

                notes += "int " + ASTHelper.PrintNode(typeName);

                if (parser.Tokenizer.Peek() != ")") {
                    notes += ", ...";
                }

                notes += ") -> ...";

                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = parser.Tokenizer.Peek(),
                    In = "function parameter list",
                    Expected = "a parameter name",
                    ExtraNotes = notes
                });

                isValid = false;
            }

        LOOP_END_CHECK: // FIXME: i know, this is bad practice, and i'm fully open to alternatives
            if (parser.Tokenizer.Consume() != ",") {
                if (parser.Tokenizer.Current == ")") {
                    parser.Tokenizer.Reconsume();

                    break;
                }

                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = parser.Tokenizer.Current,
                    In = "after a parameter in a function's parameter list",
                    Expected = "a comma or a parenthesis"
                });

                isValid = false;

                parser.Tokenizer.Reconsume();

                continue;
            }

            if (parser.Tokenizer.Peek() == ")") { // cause that would mean it is a comma followed directly by a parenthesis
                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = parser.Tokenizer.Peek(),
                    In = "after a comma in a function's parameter list",
                    Expected = "a parameter name or type. Did you forget a parameter ?",
                    Location = new LocationRange(parser.Tokenizer.Current.Location, parser.Tokenizer.Peek().Location)
                });

                isValid = false;
            }
        }

        var closingParen = parser.Tokenizer.Consume();

        // this means we probably broke because of an EOF
        if (closingParen != ")") {
            Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                In = "a function's parameter list",
                Expected = "a closing parenthesis ')'",
                Location = parser.Tokenizer.Position
            });

            isValid = false;
        }

        var returnType = NameNode.NULL;

        var colonToken = Token.NULL;

        if (parser.Tokenizer.Peek() == ":") {
            colonToken = parser.Tokenizer.Consume(); // consume the colon

            returnType = parser.ExpressionParser.Consume<NameNode>(
                IdentNode.NULL,
                @as: "a return type in a function declaration"
            );

            if (!returnType.IsValid) {
                isValid = false;

                returnType = new IdentNode(
                    new IdentToken(
                        returnType.Token.Representation,
                        returnType.Location,
                        false
                    ),
                    false
                );
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
