public class NumberNode : ValueNode
{
    /// <summary>
    /// The value of this NumberNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public double Value { get; protected set; }

    public new NumberToken Token { get; protected set; }

    public NumberNode(double value, NumberToken token, bool isValid = true) : base(value.ToString(), token, token.Location, isValid) {
        Value = value;
        Token = token;
    }

    public NumberNode(NumberToken token) : this(token.Value, token)
    { }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}