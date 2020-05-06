public class NumberNode : ValueNode
{
    /// <summary>
    /// The value of this NumberNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public double Value { get; protected set; }

    public NumberNode(double value, Token token) : base(value.ToString(), token) {
        Value = value;
    }

    public NumberNode(NumberToken token) : this(token.Value, token)
    { }
}
