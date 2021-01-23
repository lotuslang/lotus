public class IdentNode : ValueNode
{
    public new ComplexToken Token { get; }

    public string Value { get; protected set; }

    public IdentNode(string value, IdentToken identToken, bool isValid = true) : base(value, identToken, identToken.Location, isValid) {
        Token = identToken;
        Value = value;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
