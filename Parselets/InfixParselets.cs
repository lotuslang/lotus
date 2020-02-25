using System;
using System.Collections.Generic;

public class BinaryOperatorParselet : IInfixParselet
{
    public Precedence Precedence { get; }

    readonly string opType;

    public BinaryOperatorParselet(Precedence precedence, string operation) {
        Precedence = precedence;
        opType = operation;
    }

    public StatementNode Parse(Parser parser, Token token, StatementNode left) {
        if (token is OperatorToken operatorToken) {
            return new OperationNode(
                operatorToken,
                new ValueNode[] {
                    left as ValueNode,
                    parser.ConsumeValue(Precedence - (operatorToken.IsLeftAssociative ? 0 : 1))
                },
                "binary" + opType
            );
        }

        throw new ArgumentException(nameof(token) + " needs to be an operator.");
    }
}

public class FuncCallParselet : IInfixParselet
{
    public Precedence Precedence {
        get => Precedence.FuncCall;
    }

    public StatementNode Parse(Parser parser, Token leftParen, StatementNode function) {

        if (!(function is ValueNode)) {
            throw new ArgumentException(nameof(function) + " needs to be, at least, a value.");
        }

        parser.Tokenizer.Reconsume();

        var args = parser.ConsumeCommaSeparatedList("(", ")");

        return new FunctionCallNode(args, function as ValueNode, leftParen);
    }
}

public class ArrayAccessParselet : IInfixParselet
{
    public Precedence Precedence {
        get => Precedence.ArrayAccess;
    }

    public StatementNode Parse(Parser parser, Token leftSquareBracket, StatementNode array) {
        if (!(array is ValueNode)) {
            throw new ArgumentException(nameof(array) + " needs to be, at least, an expression/value.");
        }

        var index = parser.ConsumeValue();

        if (parser.Tokenizer.Consume() != "]") {
            throw new UnexpectedTokenException(parser.Tokenizer.Current, "]");
        }

        return new OperationNode(
            new OperatorToken('[', Precedence.ArrayAccess, true, leftSquareBracket.Location),
            new ValueNode[] {
                array as ValueNode,
                index
            },
            "arrayAccess"
        );
    }
}