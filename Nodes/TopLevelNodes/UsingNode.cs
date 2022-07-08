public record UsingNode(ValueNode Name, Token Token, bool IsValid = true)
: TopLevelNode(Token, new LocationRange(Token.Location, Name.Location), IsValid)
{
    public new static readonly UsingNode NULL = new(ValueNode.NULL, Token.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}