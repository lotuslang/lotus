public class TypeCastNode : ValueNode
{
    public ValueNode Type { get; protected set; }

    public ValueNode Operand { get; protected set; }

    public TypeCastNode(ValueNode type, ValueNode operand, Token parenToken) : base(parenToken) {
        Type = type;
        Operand = operand;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "type-cast");

        root.AddProperty("color", "purple");
        root.AddProperty("tooltip", "type-casting expression");

        var typeNode = Type.ToGraphNode();

        typeNode.AddProperty("tooltip", "type");

        root.AddNode(typeNode);

        var operandNode = Operand.ToGraphNode();

        operandNode.AddProperty("tooltip", "operand");

        root.AddNode(operandNode);

        return root;
    }
}
