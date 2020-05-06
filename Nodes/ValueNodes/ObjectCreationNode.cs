public class ObjectCreationNode : ValueNode
{
    public FunctionCallNode InvocationNode { get; protected set; }

    public ObjectCreationNode(FunctionCallNode invoke, ComplexToken newToken) : base(newToken) {
        InvocationNode = invoke;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "obj creation");

        root.AddProperty("color", "indigo");
        root.AddProperty("tooltip", "ctor/object creation");

        var classNameNode = InvocationNode.FunctionName.ToGraphNode();

        classNameNode.AddProperty("color", "");
        classNameNode.AddProperty("tooltip", "class name");

        root.AddNode(classNameNode);

        if (InvocationNode.CallingParameters.Count == 0) {
            root.AddNode(new GraphNode(InvocationNode.CallingParameters.GetHashCode(), "(no args)"));

            return root;
        }

        var argsNode = new GraphNode("args");

        foreach (var parameter in InvocationNode.CallingParameters)
        {
            var paramNode = parameter.ToGraphNode();

            paramNode.AddProperty("tooltip", "argument");

            argsNode.AddNode(paramNode);
        }

        root.AddNode(argsNode);

        return root;
    }
}