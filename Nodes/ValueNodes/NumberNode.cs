public class NumberNode : ValueNode
{
    public new static readonly NumberNode NULL = new NumberNode(0M, NumberToken.NULL, false);

    /// <summary>
    /// The value of this NumberNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public decimal Value { get; protected set; }

    public NumberNode(decimal value, NumberToken token, bool isValid = true) : base(value.ToString(), token, token.Location, isValid) {
        Value = value;
    }

    public NumberNode(NumberToken token) : this(token.Value, token)
    { }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}