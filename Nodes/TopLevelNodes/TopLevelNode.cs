public abstract record TopLevelNode(Token Token, LocationRange Location)
: Node(Token, Location)
{
    public new static readonly TopLevelNode NULL = new Dummy();

    protected TopLevelNode(Token token) : this(token, token.IsValid) { }
    protected TopLevelNode(Token token, bool isValid) : this(token, token.Location, isValid) { }
    protected TopLevelNode(Token token, LocationRange loc, bool isValid) : this(token, loc) {
        IsValid = isValid;
    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);

    internal record Dummy() : TopLevelNode(Token.NULL, false);
}