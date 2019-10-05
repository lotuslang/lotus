using System;
using System.Linq;
using System.Collections.Generic;

public class BinaryOperatorParselet : IInfixParselet
{
    Precedence precedence;

    public Precedence Precedence {
        get => precedence;
    }

    string opType;

    public BinaryOperatorParselet(Precedence precedence, string operation) {
        this.precedence = precedence;
        opType = operation;
    }

    public StatementNode Parse(Parser parser, Token token, StatementNode left) {
        if (token is OperatorToken operatorToken) {
            return new OperationNode(
                token,
                new ValueNode[] {
                    left as ValueNode,
                    parser.ConsumeValue((int)precedence - (operatorToken.IsLeftAssociative ? 0 : 1))
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

    public StatementNode Parse(Parser parser, Token token, StatementNode function) {
        var args = new List<ValueNode>();

        if (!(function is ValueNode)) {
            throw new ArgumentException(nameof(function) + " needs to be, at least, an expression/value.");
        }

        //parser.tokenizer.Consume();

        if (parser.Tokenizer.Peek() != ")") {
            do {
                args.Add(parser.ConsumeValue());
            } while (parser.Tokenizer.Peek() != ")");
        }

        parser.Tokenizer.Consume(); // the remaining ')'

        return new FunctionCallNode(args.ToArray(), function as ValueNode, token);
    }
}

public class ArrayAccessParselet : IInfixParselet
{
    public Precedence Precedence {
        get => Precedence.Array;
    }

    public StatementNode Parse(Parser parser, Token token, StatementNode array) {
        if (!(array is ValueNode)) {
            throw new ArgumentException(nameof(array) + " needs to be, at least, an expression/value.");
        }

        var index = parser.ConsumeValue();

        if (parser.Tokenizer.Consume() != "]") {
            throw new UnexpectedTokenException(parser.Tokenizer.Current, "]");
        }

        return new OperationNode(token, new ValueNode[] { array as ValueNode, index }, "arrayAccess");
    }
}