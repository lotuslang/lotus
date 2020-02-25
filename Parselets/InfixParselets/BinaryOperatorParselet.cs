using System;

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
