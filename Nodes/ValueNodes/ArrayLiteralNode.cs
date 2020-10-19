using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ArrayLiteralNode : ValueNode
{
    public ReadOnlyCollection<ValueNode> Content { get; }

    public Token ClosingBracket { get; }

    public ArrayLiteralNode(IList<ValueNode> content, Token leftSquareBracketToken, Token rightBracket, bool isValid = true)
    : base(leftSquareBracketToken, isValid)
    {
        Content = content.AsReadOnly();
        ClosingBracket = rightBracket;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "array literal\\n(" + Content.Count + " item(s))")
            .SetColor("teal")
            .SetTooltip("array literal");

        var itemCount = 0;

        foreach (var item in Content) root.Add(item.ToGraphNode().SetTooltip("item " + ++itemCount));

        return root;
    }
}
