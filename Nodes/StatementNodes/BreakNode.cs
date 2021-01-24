public class BreakNode : StatementNode
{
    public new static readonly BreakNode NULL = new BreakNode(ComplexToken.NULL, false);

    public BreakNode(ComplexToken breakToken, bool isValid = true) : base(breakToken, breakToken.Location, isValid) { }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}