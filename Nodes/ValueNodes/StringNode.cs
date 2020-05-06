public class StringNode : ValueNode
{
    /// <summary>
    /// The value of this StringNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public string Value { get; protected set; }

    public StringNode(string value, Token token) : base(value, token)
    {
        Value = value;
    }

    public override GraphNode ToGraphNode() {
        var output = new GraphNode(GetHashCode(), "'" + Representation.Replace(@"\", @"\\").Replace("'", @"\'").Replace("\"", "\\\"") + "'");

        output.AddProperty("color", "orange");
        output.AddProperty("tooltip", nameof(StringNode));

        return output;
    }
}
