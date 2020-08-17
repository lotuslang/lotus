using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ArrayLiteralNode : ValueNode
{
    public ReadOnlyCollection<ValueNode> Content { get; }
    public ArrayLiteralNode(IList<ValueNode> content, Token leftSquareBracketToken, bool isValid = true)
    : base(leftSquareBracketToken, isValid)
    {
        Content = content.AsReadOnly();
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "array literal\\n(" + Content.Count + " item(s))")
            .SetColor("teal")
            .SetTooltip("array init");

        var itemCounter = 1;

        foreach (var item in Content) root.Add(item.ToGraphNode().SetTooltip("item " + itemCounter++));

        return root;
    }
}
