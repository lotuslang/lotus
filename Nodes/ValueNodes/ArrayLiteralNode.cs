using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ArrayLiteralNode : ValueNode
{
    public ReadOnlyCollection<ValueNode> Content { get; }
    public ArrayLiteralNode(IList<ValueNode> content, Token leftSquareBracketToken) : base(leftSquareBracketToken) {
        Content = content.AsReadOnly();
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "array literal\\n(" + Content.Count + " item(s))");

        root.AddProperty("color", "teal");
        root.AddProperty("tooltip", "array init");

        var itemCounter = 1;

        foreach (var item in Content) {
            var itemNode = item.ToGraphNode();

            itemNode.AddProperty("tooltip", "item " + itemCounter++);

            root.AddNode(itemNode);
        }

        return root;
    }
}
