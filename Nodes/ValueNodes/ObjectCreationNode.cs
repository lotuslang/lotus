public class ObjectCreationNode : ValueNode
{
    public FunctionCallNode InvocationNode { get; protected set; }

    public ValueNode TypeName => InvocationNode.FunctionName;

    public ObjectCreationNode(FunctionCallNode invoke, ComplexToken newToken, bool isValid = true) : base(newToken, isValid) {
        InvocationNode = invoke;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "obj creation") {
            TypeName.ToGraphNode()
                .SetColor("")
                .SetTooltip("class name"),
        };

        root.SetColor("indigo")
            .SetTooltip("ctor class/object creation");

        if (InvocationNode.CallingParameters.Count == 0) {
            root.Add(new GraphNode(InvocationNode.CallingParameters.GetHashCode(), "(no args)"));

            return root;
        }

        var argsNode = new GraphNode("args");

        foreach (var parameter in InvocationNode.CallingParameters) {
            argsNode.Add(parameter.ToGraphNode().SetTooltip("argument"));
        }

        root.Add(argsNode);

        return root;
    }
}