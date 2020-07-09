using System;
using System.Linq;
using System.Collections.Generic;

public sealed class FunctionDeclarationParselet : IStatementParselet<FunctionDeclarationNode>
{
    public FunctionDeclarationNode Parse(Parser parser, Token funcToken) {

        // if the token consumed was not "func", then throw an exception
        if (!(funcToken is ComplexToken funcKeyword && funcKeyword == "func"))
            throw new UnexpectedTokenException("A function definition must start with the keyword 'func'", funcToken);

        // consume the name of the function
        var funcNameToken = parser.Tokenizer.Consume();

        // if the token consumed was not an identifier, then throw an exception
        if (!(funcNameToken is IdentToken funcName))
            throw new UnexpectedTokenException(funcNameToken, "in function declaration", TokenKind.ident);

        if (parser.Tokenizer.Consume() != "(") {
            throw new Exception();
        }

        // function parameter list format :
        // func Foo(someVar, string someStr, int [start, end]) -> int
        // { /* some code */ }
        //

        var parameters = new List<(ValueNode type, ComplexToken name)>();

        while (parser.Tokenizer.Peek() != ")") {

            /*
            * Ok, this needs a bit of explanation. So, I wanted to implement a syntactic shorthand I'd like to see
            * in csharp (and other languages) that allows users to define the type of multiple parameters at once
            * if they have the same type. It can be really long frustrating to have things like :
            *
            * ```
            * int DoThing(int seed, int cookie, int hash, int secret, int key) {
            *     return (new Random(seed).Next() * cookie ^ hash % key) & secret;
            * }
            * ```
            *
            * Therefore, I thought of a shorthand such as : `int DoThing(int [seed, cookie, hash, secret, key])`
            * As you probably noticed, it is really similar to an array init/literal, which is honestly a good
            * thing in my opinion.So I thought I would just parse the type, and then, if the next node wasn't an
            * IdentNode, I would just use the ArrayLiteral parselet and register each parameter with the type.
            *
            * Yeah, that's not what happened.
            *
            * What I didn't notice is that int [a, b, c] is actually an array indexing, you just have to remove the
            * space and it becomes quite apparent. I had to make array accesses able to handle commas, but I'm not
            * happy about it because it made more to sense (to me) and was cleaner to parse the type first and then
            * an array of names. But alas, I have to do this for now, and I don't think I could easily fix this without
            * re-writing a special parsing algorithm in this method just for dots and parentheses.
            */

            var typeOrName = parser.ConsumeValue();

            if (typeOrName is OperationNode typeNameArray) {
                if (typeNameArray.OperationType != OperationType.ArrayAccess || typeNameArray.Operands.Count < 2) {
                    throw new Exception();
                }

                var paramType = typeNameArray.Operands[0];

                if (!Utilities.IsName(paramType)) {
                    throw new Exception();
                }

                // we skip one cause the first operand is the type name
                foreach (var nameNode in typeNameArray.Operands.Skip(1)) {
                    if (!(nameNode is IdentNode paramNode)) {
                        throw new Exception();
                    }

                    parameters.Add((paramType, paramNode.Token));
                }
            } else if (parser.Tokenizer.Peek() != "," && parser.Tokenizer.Peek() != ")") {
                if (!Utilities.IsName(typeOrName)) {
                    throw new Exception();
                }

                if (!(parser.Peek() is IdentNode nameNode)) {
                    throw new Exception();
                }

                parameters.Add((typeOrName, nameNode.Token));
            } else {
                if (!(typeOrName is IdentNode paramName)) {
                    throw new Exception();
                }

                parameters.Add((ValueNode.NULL, paramName.Token));
            }

            if (parser.Tokenizer.Consume() != ",") {
                if (parser.Tokenizer.Current == ")") {
                    parser.Tokenizer.Reconsume();

                    break;
                }

                throw new Exception(); // TODO: Write exceptions
            }

            if (parser.Tokenizer.Peek() == ")") { // cause that would mean it is a comma followed directly by a paren
                throw new Exception();
            }
        }

        if (parser.Tokenizer.Consume() != ")") {
            throw new Exception("wtf did you break");
        }

        var returnType = ValueNode.NULL;

        if (parser.Tokenizer.Peek() == ":") {
            parser.Tokenizer.Consume(); // consume the arrow

            returnType = parser.ConsumeValue();

            if (!Utilities.IsName(returnType)) {
                throw new Exception();
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
            funcKeyword
        );
    }
}
