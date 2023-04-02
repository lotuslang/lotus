namespace Lotus.Extras.Graphs;

public static class Edge
{
    public static void AppendEdgeBetween(IndentedStringBuilder sb, GraphNode n1, GraphNode n2)
        => sb.Append(n1.ID).Append(" -- ").Append(n2.ID).AppendLine();

    public static void AppendEdgeBetween(IndentedStringBuilder sb, Graph graph, GraphNode node) {
        EnsureNonEmptyCluster(graph);

        sb
            .Append(GetMiddleNode(graph).ID)
            .Append(" -- ")
            .Append(node.ID)
            .Append(" [ltail=cluster_")
            .Append(graph.ID)
            .Append(']')
            .AppendLine();
    }

    public static void AppendEdgeBetween(IndentedStringBuilder sb, GraphNode node, Graph graph) {
        EnsureNonEmptyCluster(graph);

        sb
            .Append(node.ID)
            .Append(" -- ")
            .Append(GetMiddleNode(graph).ID)
            .Append(" [lhead=cluster_")
            .Append(graph.ID)
            .Append(']')
            .AppendLine();
    }

    public static void AppendEdgeBetween(IndentedStringBuilder sb, Graph g1, Graph g2) {
        EnsureNonEmptyCluster(g1);
        EnsureNonEmptyCluster(g2);

        sb
            .Append(GetMiddleNode(g1).ID)
            .Append(" -- ")
            .Append(GetMiddleNode(g2).ID)
            .Append(" [ltail=cluster_")
            .Append(g1.ID)
            .Append(", lhead=cluster_")
            .Append(g2.ID)
            .Append(']')
            .AppendLine();
    }

    private static GraphNode GetMiddleNode(Graph g) {
        Debug.Assert(g.RootNodes.Count != 0);

        return g.RootNodes[g.RootNodes.Count / 2];
    }

    private static void EnsureNonEmptyCluster(Graph g) {
        if (g.RootNodes.Count != 0)
            return;

        g.Add(new GraphNode("_ghost").SetProperty("style", "invis"));
    }
}