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
        if (!(funcNameToken is ComplexToken funcName && funcName == TokenKind.ident))
            throw new UnexpectedTokenException(funcNameToken, "in function declaration", TokenKind.ident);

        if (parser.Tokenizer.Consume() != "(") {
            throw new Exception();
        }

        // function parameter list format :
        // func Foo(someVar, string someStr, ...) -> int
        // { /* some code */ }
        //

        var parameters = new List<(ValueNode type, ComplexToken name)>();

        while (parser.Tokenizer.Peek() != ")") {
            var typeOrName = parser.ConsumeValue();

            ValueNode paramType;
            ComplexToken paramName;

            if (parser.Tokenizer.Peek() != "," && parser.Tokenizer.Peek() != ")") {
                if (!Utilities.IsName(typeOrName)) {
                    throw new Exception();
                }

                paramType = typeOrName;

                if (!(parser.ConsumeValue().Token is ComplexToken paramNameToken)) {
                    throw new Exception();
                }

                paramName = paramNameToken;
            } else {
                paramType = ValueNode.NULL;

                if (!(typeOrName.Token is ComplexToken paramNameToken)) {
                    throw new Exception();
                }

                paramName = paramNameToken;
            }

            parameters.Add((paramType, paramName));

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
