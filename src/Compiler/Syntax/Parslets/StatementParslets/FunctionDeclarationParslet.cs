namespace Lotus.Syntax;

public sealed class FunctionDeclarationParslet : IStatementParslet<FunctionDeclarationNode>
{
    public static readonly FunctionDeclarationParslet Instance = new();

    private static readonly ValueTupleParslet<FunctionParameter> _paramListParslet
        = new(ParseFuncParam) {
            Start = "(",
            End = ")",
            EndingDelimBehaviour = TupleEndingDelimBehaviour.Reject,
        };

    public FunctionDeclarationNode Parse(StatementParser parser, Token funcToken) {
        Debug.Assert(funcToken == "func");

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

            funcName = new IdentToken(funcNameToken.Representation, funcNameToken.Location) { IsValid = false };
        }

        var paramList = _paramListParslet.Parse(parser.ExpressionParser);

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
                        returnType.Location
                    ) { IsValid = false }
                );
            }
        }

        // consume a simple block
        var block = parser.ConsumeStatementBlock(areOneLinersAllowed: false);

        // return a new FunctionDeclarationNode with 'block' as the value, 'parameters' as the list of params, and funcName as the name
        return new FunctionDeclarationNode(
            block,
            paramList,
            returnType,
            funcName,
            funcToken,
            colonToken
        ) { IsValid = isValid };
    }

    static FunctionParameter MakeFuncParameter(ValueNode type, ValueNode paramNameNode) {
        bool isValid = true;

        if (type is not NameNode typeName) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = type,
                As = "a parameter type in a function's parameter list",
                Expected = "a type name (a qualified name)"
            });

            isValid = false;

            typeName = NameNode.NULL;
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
                _ = Logger.errorStack.Pop();
            }

            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = paramNameNode,
                As = "a parameter name in a function's parameter list",
                Expected = "a simple name (an identifier)"
            });

            isValid = false;

            paramName = new IdentNode(
                new IdentToken(
                    paramNameNode.Token.Representation,
                    paramNameNode.Token.Location
                ) { IsValid = false }
            );
        }

        return new FunctionParameter(typeName, paramName, defaultValue, equalSign) { IsValid = isValid };
    }

    static IEnumerable<FunctionParameter> ParseFuncParam(ExpressionParser parser) {
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

        var typeName = parser.Consume();

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

                // fixme(logging): Maybe we could try to consume until there's either a comma or a parenthesis,
                // but if there isn't any (i.e. the function is malformed later on), we would have a problem :/

                yield return FunctionParameter.NULL;
                yield break;
            }

            var isValid = true;
            var paramType = typeNameArray.Operands.FirstOrDefault(ValueNode.NULL);

            // if there's less than 2 names (illegal)
            if (typeNameArray.Operands.Length < 2) { // fixme(utils): We should really make a helper for wrong-numbered things
                var errorLoc = paramType.Location;

                if (typeNameArray.Operands.Length == 0) {
                    errorLoc =
                        new LocationRange(
                            typeNameArray.AdditionalTokens[0].Location,
                            typeNameArray.AdditionalTokens[1].Location
                        );
                }

                Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                    Value = typeNameArray.Operands[0],
                    Location = errorLoc,
                    Message = "A typed parameter-group should declare at least two parameters, which is not the case here.",
                });

                isValid = false;
            }

            // iterate over every parameter name and add them to the list
            // we skip one cause the first operand of the array access is the type name
            foreach (var paramNameNode in typeNameArray.Operands.Skip(1)) {
                yield return MakeFuncParameter(paramType ?? ValueNode.NULL, paramNameNode) with { IsValid = isValid };
            }
        } else if (parser.Tokenizer.Peek().Kind == TokenKind.identifier) {
            // if there's still an identifier after typeOrName,
            // it's a typed parameter (and the name is the token we peeked at)

            yield return MakeFuncParameter(typeName, parser.Consume());
        } else {
            // otherwise, that means we have a parameter without type info
            //! in that case, typeName is probably a parameter name

            // add it anyway for consistency and in case the param name
            // isn't even a name, because addParameter handles that for us
            var param = MakeFuncParameter(NameNode.NULL, typeName);

            // if the param name wasn't valid, no need to emit a second misleading
            // error
            if (!param.IsValid) {
                yield return FunctionParameter.NULL;
                yield break;
            }

            var notes = "Did you forget to specify a type for this parameter ? "
                      + "For example, you could write:\n\t"
                      + "..., int "+ ASTUtils.PrintValue(typeName) + ", ...";

            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = parser.Tokenizer.Peek(),
                In = "function parameter list",
                Expected = "a parameter name",
                ExtraNotes = notes
            });

            yield return param;
        }

        yield break;
    }
}
