internal sealed class NameChecker : IValueVisitor<bool>
{
    public bool Default(ValueNode node) => false;

    public bool Visit(OperationNode node) {
        if (node.OperationType == OperationType.Access) {
            if (node.Operands.Count != 2) return false;

            // we only need to check the left-hand operand, because we know the right-hand operand
            // is an IdentNode, because an access operation is defined as :
            //  access-operation :
            //      value '.' identifier
            // hence, the left-hand side
            return node.Operands[0].Accept(this);
        }

        return false;
    }

    public bool Visit(IdentNode node) => true;

    public bool Visit(ParenthesizedValueNode node) => node.Values.Accept(this);

    public bool Visit(SimpleBlock block) => false;

    public bool IsName(ValueNode node) => node.Accept(this);
}