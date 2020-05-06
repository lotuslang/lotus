using System.Collections.Generic;
using System.Collections.ObjectModel;

public class FunctionCallNode : ValueNode
{
    public ReadOnlyCollection<ValueNode> CallingParameters { get; }

    public ValueNode FunctionName { get; protected set; }

    public FunctionCallNode(IList<ValueNode> parameters, ValueNode functionName, Token functionToken)
        : base(functionName.Representation + "(...)", functionToken)
    {

        FunctionName = functionName;
        CallingParameters = parameters.AsReadOnly();
    }

    public override GraphNode ToGraphNode() {

        GraphNode root;

        if (FunctionName is IdentNode name) {
            root = new GraphNode(GetHashCode(), name.Representation + "(...)");

        } else {
            root = new GraphNode(GetHashCode(), "call");

            var called = new GraphNode("function");

            called.AddNode(FunctionName.ToGraphNode());

            root.AddNode(called);
        }

        root.AddProperty("color", "tomato");
        root.AddProperty("tooltip", "call to " + nameof(FunctionCallNode));

        if (CallingParameters.Count == 0) {
            root.AddNode(new GraphNode(CallingParameters.GetHashCode(), "(no args)"));

            return root;
        }

        var argsNode = new GraphNode("args");

        foreach (var parameter in CallingParameters)
        {
            var paramNode = parameter.ToGraphNode();

            paramNode.AddProperty("tooltip", "argument");

            argsNode.AddNode(paramNode);
        }

        root.AddNode(argsNode);

        return root;
    }
}
