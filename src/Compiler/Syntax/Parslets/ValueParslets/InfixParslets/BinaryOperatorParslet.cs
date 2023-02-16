namespace Lotus.Syntax;

public sealed class BinaryOperatorParslet : IInfixParslet<OperationNode>
{
    public Precedence Precedence { get; }

    private readonly OperationType _opType;

    public BinaryOperatorParslet(Precedence precedence, OperationType operation) {
        Precedence = precedence;
        _opType = operation;
    }

    public OperationNode Parse(Parser parser, Token token, ValueNode left) {
        var operatorToken = token as OperatorToken;

        Debug.Assert(operatorToken is not null);

        return new OperationNode(
            operatorToken,
            ImmutableArray.Create(
                left,
                parser.ConsumeValue(Precedence - (operatorToken.IsLeftAssociative ? 0 : 1)) // still is magic to me
            ),
            _opType
        );
    }
}
