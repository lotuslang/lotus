using System;
using System.Text;
using System.Collections.Generic;

public static class DotGen
{
    public static GraphNode ToGraphNode(this ValueNode node) {
        var root = new GraphNode(node.GetHashCode().ToString(), "\"" + node.Representation + "\"");

        if (node is OperationNode) {
            if (node.Representation == "++" || node.Representation == "--") {
                root = new GraphNode(node.GetHashCode().ToString(), ((node as OperationNode).OperationType.EndsWith("Post") ? "\"(postfix)" : "\"(prefix)") + node.Representation + "\"");
            }

            if (node.Representation == "_") {
                root = new GraphNode(node.GetHashCode().ToString(), "\"-\"");
            }

            foreach (var child in (node as OperationNode).Operands)
            {
                root.AddNode(ToGraphNode(child));
            }
        }

        return root;
    }
}