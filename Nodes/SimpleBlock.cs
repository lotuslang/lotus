using System.Collections.Generic;
using System.Collections.ObjectModel;

public class SimpleBlock
{
    public ReadOnlyCollection<StatementNode> Content { get; protected set; }

    public bool IsValid { get; set; }

    public SimpleBlock(IList<StatementNode> content, bool isValid = true) {
        Content = content.AsReadOnly();

        IsValid = isValid;
    }

    public GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "block")
            .SetColor("darkviolet")
            .SetTooltip(nameof(SimpleBlock));

        foreach (var statement in Content) root.Add(statement.ToGraphNode());

        return root;
    }
}