public record UsingNode(Union<StringNode, NameNode> Name, Token Token, bool IsValid = true)
: TopLevelNode(Token, new LocationRange(Token.Location, Name.Match(s => s.Location, n => n.Location)), IsValid)
{
    public new static readonly UsingNode NULL = new(StringNode.NULL, Token.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}