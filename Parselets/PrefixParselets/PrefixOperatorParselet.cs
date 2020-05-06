using System;

public sealed class PrefixOperatorParselet : IPrefixParselet<OperationNode>
{
    readonly OperationType opType;

    public PrefixOperatorParselet(OperationType operation) {
        opType = operation;
    }

    public OperationNode Parse(Parser parser, Token token) {

        if (!(token is OperatorToken opToken)) throw new Exception();

        return new OperationNode(opToken, new ValueNode[] { parser.ConsumeValue(Precedence.Unary) }, opType);
    }
}