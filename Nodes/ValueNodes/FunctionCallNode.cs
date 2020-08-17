using System.Collections.Generic;
using System.Collections.ObjectModel;

public class FunctionCallNode : ValueNode
{
    public ReadOnlyCollection<ValueNode> CallingParameters { get; }

    public ValueNode FunctionName { get; protected set; }

    public FunctionCallNode(IList<ValueNode> parameters, ValueNode functionName, Token functionToken, bool isValid = true)
        : base(functionName.Representation + "(...)", functionToken, isValid)
    {

        FunctionName = functionName;
        CallingParameters = parameters.AsReadOnly();
    }

    public override GraphNode ToGraphNode() {

        GraphNode root;

        if (FunctionName is IdentNode name) {
            root = new GraphNode(GetHashCode(), name.Representation + "(...)");
        } else {
            root = new GraphNode(GetHashCode(), "call") {
                new GraphNode("function") {
                    FunctionName.ToGraphNode()
                },
            };
        }

        root.SetColor("tomato")
            .SetTooltip("call to " + nameof(FunctionCallNode));

        if (CallingParameters.Count == 0) {
            root.Add(new GraphNode(CallingParameters.GetHashCode(), "(no args)"));

            return root;
        }

        var argsNode = new GraphNode("args");

        foreach (var parameter in CallingParameters) argsNode.Add(parameter.ToGraphNode().SetTooltip("argument"));

        root.Add(argsNode);

        return root;
    }
}
