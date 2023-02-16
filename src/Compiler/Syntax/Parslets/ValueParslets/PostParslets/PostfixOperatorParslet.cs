namespace Lotus.Syntax;

public sealed class PostfixOperatorParslet : IPostfixParslet<OperationNode>
{
    public Precedence Precedence => Precedence.Unary;

    private readonly OperationType _opType;

    public PostfixOperatorParslet(OperationType operation) {
        _opType = operation;
    }

    public OperationNode Parse(Parser parser, Token token, ValueNode left) {
        var operatorToken = token as OperatorToken;

        Debug.Assert(operatorToken is not null);

        return new OperationNode(
            operatorToken,
            ImmutableArray.Create(left),
            _opType
        );
    }
}