using System;

public class PostfixOperatorParselet : IPostfixParselet
{
    public Precedence Precedence { get; }

    readonly string opType;

    public PostfixOperatorParselet(string operation) {
        Precedence = Precedence.Unary;
        opType = operation;
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