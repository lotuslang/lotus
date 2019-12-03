using System;
using System.Linq;
using System.Collections.Generic;

public class PostfixOperatorParselet : IPostfixParselet
{
    Precedence precedence;

    public Precedence Precedence {
        get => precedence;
    }

    string opType;

    public PostfixOperatorParselet(string operation) {
        this.precedence = Precedence.Unary;
        this.opType = operation;
    }

    public StatementNode Parse(Parser parser, Token token, StatementNode left) {
        if (token is OperatorToken operatorToken) {
            return new OperationNode(
                operatorToken,
                new ValueNode[] {
                    left as ValueNode
                },
                "postfix" + opType
            );
        }

        throw new ArgumentException(nameof(token) + " needs to be an operator.");
    }
}