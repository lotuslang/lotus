using System;
using System.Linq;

public sealed class FunctionDeclarationParselet : IStatementParselet<FunctionDeclarationNode>
{
    public FunctionDeclarationNode Parse(Parser parser, Token defToken) {

        // if the token consumed was not "def", then throw an exception
        if (!(defToken is ComplexToken defKeyword && defKeyword == "def"))
            throw new UnexpectedTokenException("A function definition must start with the keyword 'def'", defToken);

        // consume the name of the function
        var funcNameToken = parser.Tokenizer.Consume();

        // if the token consumed was not an identifier, then throw an exception
        if (!(funcNameToken is ComplexToken funcName && funcName == TokenKind.ident))
            throw new UnexpectedTokenException(funcNameToken, "in function declaration", TokenKind.ident);

        var parameters = parser.ConsumeCommaSeparatedValueList("(", ")");

        // yes this returns true even if the parameter list is empty
        if (!parameters.All(parameter => parameter.Token is ComplexToken))
            throw new UnexpectedTokenException(
                parameters.Find(p => !(p.Token is ComplexToken))!.Token,
                "in a declared function parameters list",
                TokenKind.ident
            );

        // consume a simple block
        var block = parser.ConsumeSimpleBlock();

        // return a new FunctionDeclarationNode with 'block' as the value, 'parameters' as the list of params, and funcName as the name
        return new FunctionDeclarationNode(
            block,
            parameters.Select(parameter => parameter.Token as ComplexToken)!.ToArray()!,
            funcName,
            defKeyword
        );
    }
}
