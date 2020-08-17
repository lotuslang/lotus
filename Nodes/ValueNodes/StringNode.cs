public class StringNode : ValueNode
{
    /// <summary>
    /// The value of this StringNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public string Value { get; protected set; }

    public StringNode(string value, Token token, bool isValid = true) : base(value, token, isValid)
    {
        Value = value;
    }

    public override GraphNode ToGraphNode()
        => new GraphNode(GetHashCode(), "'" + Representation.Replace(@"\", @"\\").Replace("'", @"\'").Replace("\"", "\\\"") + "'")
            .SetColor("orange")
            .SetTooltip(nameof(StringNode));
}
