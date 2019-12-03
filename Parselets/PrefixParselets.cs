using System;
using System.Linq;
using System.Collections.Generic;

public class NumberParselet : IPrefixParselet
{
    public StatementNode Parse(Parser parser, Token token) {
        if (token is NumberToken number) {
            return new NumberNode(number);
        }

        throw new ArgumentException(nameof(token) + " needs to be a number.");
    }
}

public class IdentifierParselet : IPrefixParselet
{
    public StatementNode Parse(Parser parser, Token token) {
        if (token.Kind == TokenKind.ident) {
            return new IdentNode(token.Representation, token);
        }

        throw new ArgumentException(nameof(token) + " needs to be an identifier.");
    }
}

public class StringParselet : IPrefixParselet
{
    public StatementNode Parse(Parser parser, Token token) {
        if (token.Kind == TokenKind.@string) {
            return new StringNode(token.Representation, token);
        }

        if (token is ComplexStringToken complexString) {
            var node = new ComplexStringNode(complexString, new List<ValueNode>());

            foreach (var section in complexString.CodeSections) {
                node.AddSection(new Parser(new Consumer<Token>(section)).ConsumeValue());
            }

            return node;
        }

        throw new ArgumentException(nameof(token) + " needs to be a string.");
    }
}

public class BoolParselet : IPrefixParselet
{
    public StatementNode Parse(Parser parser, Token token) {
        if (token is BoolToken boolean) {
            return new BoolNode(boolean.Value, boolean);
        }

        throw new ArgumentException(nameof(token) + " needs to be a bool.");
    }
}

public class PrefixOperatorParselet : IPrefixParselet
{
    string opType;

    public PrefixOperatorParselet(string operation) {
        opType = operation;
    }

    public StatementNode Parse(Parser parser, Token token) {
        return new OperationNode(token as OperatorToken, new ValueNode[] { parser.ConsumeValue() }, "prefix" + opType);
    }
}

public class GroupParenParselet : IPrefixParselet
{
    public StatementNode Parse(Parser parser, Token token) {
        var value = parser.ConsumeValue();

        if (parser.Tokenizer.Consume() != ")") {
            throw new UnexpectedTokenException(parser.Tokenizer.Current, ")");
        }

        return value;
    }
}