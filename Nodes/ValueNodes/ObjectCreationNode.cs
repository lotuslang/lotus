public record ObjectCreationNode(FunctionCallNode Invocation, Token Token, bool IsValid = true)
: ValueNode(Token, new LocationRange(Token.Location, Invocation.Location), IsValid)
{
    public new static readonly ObjectCreationNode NULL = new(FunctionCallNode.NULL, Token.NULL, false);

    public ValueNode TypeName => Invocation.Name;

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}