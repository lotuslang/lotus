namespace Lotus.Syntax;

public sealed class PrefixOperatorParslet : IPrefixParslet<OperationNode>
{
    private readonly OperationType _opType;

    public PrefixOperatorParslet(OperationType operation) {
        _opType = operation;
    }

    public OperationNode Parse(Parser parser, Token token) {
        var opToken = token as OperatorToken;

        Debug.Assert(opToken is not null);

        return new OperationNode(opToken, ImmutableArray.Create(parser.ConsumeValue(Precedence.Unary)), _opType);
    }
}