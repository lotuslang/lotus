namespace Lotus.Syntax;

public sealed record UsingNode(Union<StringNode, NameNode> Name, Token Token)
: TopLevelNode(Token, new LocationRange(Token.Location, Name.Match(s => s.Location, n => n.Location)))
{
    public new static readonly UsingNode NULL = new(StringNode.NULL, Token.NULL) { IsValid = false };

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}