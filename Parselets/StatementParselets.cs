using System;
using System.Linq;
using System.Collections.Generic;

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

public class ReturnParselet : IStatementParselet
{
    public StatementNode Parse(Parser parser, Token returnKeyword) {

        if (returnKeyword != "return") throw new UnexpectedTokenException(returnKeyword, "in return statement", "return");

        if (parser.Tokenizer.Peek() == ";") return new ReturnNode(ValueNode.NULL, returnKeyword as ComplexToken);

        var value = parser.ConsumeValue();

        return new ReturnNode(value, returnKeyword as ComplexToken);
    }
}

public class DeclarationParselet : IStatementParselet
{
    public StatementNode Parse(Parser parser, Token varKeyword) {

        // if the token isn't the keyword "var", throw an exception
        if (varKeyword != "var") throw new UnexpectedTokenException(varKeyword, "in declaration", "var");

        // consume the name of the variable we're declaring
        var name = parser.Tokenizer.Consume();

        // if the token isn't an identifier, throw an exception
        if (name != TokenKind.ident) throw new UnexpectedTokenException(name, TokenKind.ident);

        // consume a token
        var equalSign = parser.Tokenizer.Consume();

        // if this token wasn't an equal sign, throw an exception
        if (equalSign != "=") throw new UnexpectedTokenException(equalSign, "=");

        // consume a ValueNode (which is the value of the variable we're declaring)
        var value = parser.ConsumeValue();

        // return that value
        return new DeclarationNode(value, name as ComplexToken);
    }
}

public class NamespaceParselet : IStatementParselet
{
    public StatementNode Parse(Parser parser, Token token) {
        var name = parser.ConsumeValue();

        if (!(token is ComplexToken namespaceToken && token == "namespace")) {
            throw new Exception();
        }

        if (!Utilities.IsName(name)) {
            throw new Exception();
        }

        return new NamespaceNode(name, namespaceToken);
    }
}

public class ImportParselet : IStatementParselet
{
    public StatementNode Parse(Parser parser, Token token) {
        if (!(token is ComplexToken fromKeyword && token == "from")) {
            throw new Exception();
        }

        var fromOrigin = parser.ConsumeValue();

        FromNode from;

        if (fromOrigin is IdentNode) {
            from = new FromNode(fromOrigin, fromKeyword, true);
        } else if (fromOrigin is StringNode str) {
            from = new FromNode(str, fromKeyword);
        } else {
            throw new Exception();
        }

        // TODO: Would it be better to have parser.ConsumeValue() here ? it would probably do the same thing
        //
        // For now, I think it's better to use parser.Tokenizer because it better represents what is needed here :
        // we don't need a value, we need a token. The context is important, and calling ConsumeValue() would ignore
        // that context, whereas if we consume it here, the context is clear
        //
        // The problem is that I don't like exposing the Tokenizer to the public, because that is supposed to be an "implementation
        // detail", and it's kinda against the encapsulation principle (but I mean at this point this whole thing could probably
        // be written in weird F# since there's so much immutability. We only need OOP for the Tokens, Nodes, Parselets and Toklets.
        // Because OOP is way easier to use to create a SyntaxTree and it's also the model i'm the most comfortable with)
        var importToken = parser.Tokenizer.Consume();

        if (!(importToken is ComplexToken importKeyword && token == "import")) {
            throw new Exception();
        }

        var importList = new List<ValueNode>();

        do {
            var import = parser.ConsumeValue();

            if (!Utilities.IsName(import)) throw new Exception();

            importList.Add(import);
        } while (parser.Tokenizer.Peek() == ",");

        return new ImportNode(importList, from, importKeyword);
    }
}