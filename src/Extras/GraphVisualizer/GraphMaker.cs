namespace Lotus.Extras.Graphs;

internal sealed partial class GraphMaker
{
    private static readonly (string tooltip, string color) Tuple = ("tuple", "");

    public Graph ToCluster<TVal>(Syntax.Tuple<TVal> tuple) where TVal : Node {
        var tupleGraph = new Graph("tuple");

        foreach (var item in tuple.Items)
            tupleGraph.Add(ExtraUtils.ToGraphNode(item));

        return tupleGraph;
    }

    public GraphNode ToGraphNode<TVal>(Syntax.Tuple<TVal> tuple) where TVal : Node {
        var root = new GraphNode("tuple")
            .SetColor(Tuple.color)
            .SetTooltip(Tuple.tooltip);

        foreach (var node in tuple.Items)
            root.Add(ExtraUtils.ToGraphNode(node));

        return root;
    }
}