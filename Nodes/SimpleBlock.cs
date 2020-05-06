using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class SimpleBlock
{
    public ReadOnlyCollection<StatementNode> Content { get; protected set; }

    public SimpleBlock(IList<StatementNode> content) {
        Content = content.AsReadOnly();
    }

    public GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "block");

        root.AddProperty("color", "darkviolet");
        root.AddProperty("tooltip", nameof(SimpleBlock));

        foreach (var statement in Content) {
            var statementNode = statement.ToGraphNode();

            root.AddNode(statementNode);
        }

        return root;
    }
}