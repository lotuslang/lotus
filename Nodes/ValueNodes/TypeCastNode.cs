public class TypeCastNode : ValueNode
{
    public ValueNode Type { get; protected set; }

    public ValueNode Operand { get; protected set; }

    public TypeCastNode(ValueNode type, ValueNode operand, Token parenToken) : base(parenToken) {
        Type = type;
        Operand = operand;
    }

    public override GraphNode ToGraphNode()
        => new GraphNode(GetHashCode(), "type-cast") {
            Type.ToGraphNode()
                .SetTooltip("type"),
            Operand.ToGraphNode()
                .SetTooltip("operand"),
        }.SetColor("purple")
         .SetTooltip("type-casting expression");
}
