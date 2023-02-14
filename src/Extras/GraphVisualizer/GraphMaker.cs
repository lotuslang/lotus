namespace Lotus.Extras.Graphs;

internal sealed partial class GraphMaker
{
    private static readonly (string tooltip, string color) Tuple = ("tuple", "");

    public GraphNode ToGraphNode<TVal>(Syntax.Tuple<TVal> tuple) where TVal : Node {
        var root = new GraphNode(tuple.GetHashCode(), "tuple")
            .SetColor(Tuple.color)
            .SetTooltip(Tuple.tooltip);

        foreach (var node in tuple.Items)
            root.Add(ExtraUtils.ToGraphNode(node));

        return root;
    }
}