public class BoolNode : ValueNode
{
    /// <summary>
    /// The value of this StringNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public bool Value { get; protected set; }

    public BoolNode(bool value, Token boolToken) : base(value.ToString().ToLower(), boolToken)
    {
        Value = value;
    }

    public BoolNode(string repr, Token boolToken) : this(repr == "true", boolToken)
    { }
}
