public abstract record TopLevelNode(Token Token, LocationRange Location, bool IsValid = true) : Node(Token, Location, IsValid)
{
    public new static readonly TopLevelNode NULL = new Dummy();

    protected TopLevelNode(Token token, bool isValid = true) : this(token, token.Location, isValid) { }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);

    internal record Dummy() : TopLevelNode(Token.NULL, false);
}