using System;

public sealed class BinaryOperatorParselet : IInfixParselet<OperationNode>
{
    public Precedence Precedence { get; }

    readonly OperationType opType;

    public BinaryOperatorParselet(Precedence precedence, OperationType operation) {
        Precedence = precedence;
        opType = operation;
    }

    public OperationNode Parse(Parser parser, Token token, ValueNode left) {
        if (token is OperatorToken operatorToken) {
            return new OperationNode(
                operatorToken,
                new ValueNode[] {
                    left,
                    parser.ConsumeValue(Precedence - (operatorToken.IsLeftAssociative ? 0 : 1)) // still is magic to me
                },
                opType
            );
        }

        throw new ArgumentException("Token needs to be an operator.");
    }
}
