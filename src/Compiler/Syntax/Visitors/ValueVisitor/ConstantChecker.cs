namespace Lotus.Syntax.Visitors;

internal sealed class ConstantChecker : IValueVisitor<bool>
{
    public bool Default(ValueNode node)
        => false;

    public bool Visit(ComplexStringNode node)
        => node.CodeSections.All(IsContant);

    public bool Visit(BoolNode node)
        => true;

    public bool Visit(CharNode node)
        => true;

    public bool Visit(NumberNode node)
        => true;

    public bool Visit(OperationNode node) {
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

    public bool Visit(ParenthesizedValueNode node)
        => IsContant(node.Value);

    public bool Visit(StringNode node)
        => true;

    public bool Visit(TupleNode node)
        => node.Items.All(IsContant);

    public bool IsContant(ValueNode node) => node.Accept(this);
}