public class NumberNode : ValueNode
{
    public new static readonly NumberNode NULL = new(0d, NumberToken.NULL, false);

    /// <summary>
    /// The value of this NumberNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public double Value { get; protected set; }

    public NumberNode(double value, NumberToken token, bool isValid = true) : base(token, token.Location, isValid) {
        Value = value;
        Token = token;
    }

    public NumberNode(NumberToken token) : this(token.Value, token)
    { }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}