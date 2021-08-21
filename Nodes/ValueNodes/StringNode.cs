public class StringNode : ValueNode
{
    public new static readonly StringNode NULL = new("", Token.NULL, false);
    /// <summary>
    /// The value of this StringNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public string Value { get; protected set; }

    public StringNode(string value, Token token, bool isValid = true) : base(value, token, token.Location, isValid) {
        Value = value;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
