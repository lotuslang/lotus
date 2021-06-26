public class TopLevelNode : Node
{
    public new static readonly TopLevelNode NULL = new TopLevelNode(Token.NULL, LocationRange.NULL, false);


    public TopLevelNode(Token token, LocationRange location, bool isValid = true) : base(token, location, isValid) { }
    public TopLevelNode(Token token, bool isValid = true) : base(token, isValid) { }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}