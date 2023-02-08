namespace Lotus.Syntax;

public sealed record ObjectCreationNode(FunctionCallNode Invocation, Token Token)
: ValueNode(Token, new LocationRange(Token.Location, Invocation.Location))
{
    public new static readonly ObjectCreationNode NULL = new(FunctionCallNode.NULL, Token.NULL) { IsValid = false };

    public ValueNode TypeName => Invocation.Name;

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IValueVisitor<T> visitor) => visitor.Visit(this);
}