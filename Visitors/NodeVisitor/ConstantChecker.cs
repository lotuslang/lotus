using System.Linq;

public sealed class ConstantChecker : NodeVisitor<bool>
{
    protected override bool Default(StatementNode node)
        => throw Logger.Fatal(new InvalidCallException("ConstantChecker cannot check statement like " + node.GetType().Name, node.Token.Location));


    protected override bool Default(ValueNode node)
        => false;



    // is it really useful ? i mean, you shouldn't use statements with this anyway, so...
    public override bool Visit(StatementExpressionNode node) => IsContant(node.Value);

    public override bool Visit(ComplexStringNode node)
        => node.CodeSections.All(IsContant);

    public override bool Visit(BoolNode node)
        => true;

    public override bool Visit(NumberNode node)
        => true;

    public override bool Visit(OperationNode node) {
        switch (node.OperationType) {
            case OperationType.PrefixIncrement:
            case OperationType.PostfixIncrement:
            case OperationType.PrefixDecrement:
            case OperationType.PostfixDecrement:
            case OperationType.Access:
            case OperationType.Assign:
                return false;
            default:
                return node.Operands.All(IsContant);
        }
    }

    public override bool Visit(ParenthesizedValueNode node)
        => IsContant(node.Value);

    public override bool Visit(StringNode node)
        => true;

    public override bool Visit(TupleNode node)
        => node.Values.All(IsContant);


    public override bool Visit(SimpleBlock block)
        => throw Logger.Fatal(new InvalidCallException("ConstantChecker cannot check blocks", block.Location));

    public bool IsContant(ValueNode node) => node.Accept(this);
}