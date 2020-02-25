using System;
using System.Linq;

public class FunctionDeclarationParselet : IStatementParselet
{
    public StatementNode Parse(Parser parser, Token defKeyword) {

        // if the token consumed was not "def", then throw an exception
        if (defKeyword != "def") throw new UnexpectedTokenException("A function definition must start with the keyword 'def'", defKeyword);

        // consume the name of the function
        var funcName = parser.Tokenizer.Consume();

        // if the token consumed was not an identifier, then throw an exception
        if (funcName != TokenKind.ident) throw new UnexpectedTokenException(funcName, "in function declaration", TokenKind.ident);

        var parameters = parser.ConsumeCommaSeparatedList("(", ")");

        if (!parameters.All(parameter => parameter.Token is ComplexToken)) {
            throw new Exception();
        }

        // consume a simple block
        var block = parser.ConsumeSimpleBlock();

        // return a new FunctionDeclarationNode with 'block' as the value, 'parameters' as the list of params, and funcName as the name
        return new FunctionDeclarationNode(block, parameters.Select(parameter => parameter.Token as ComplexToken).ToArray(), funcName as ComplexToken);
    }
}
