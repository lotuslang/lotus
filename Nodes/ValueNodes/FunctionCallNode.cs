public record FunctionCallNode(TupleNode ArgList, ValueNode Name, bool IsValid = true)
: ValueNode(Name.Token, new LocationRange(Name.Location, ArgList.Location), IsValid)
{
    public new static readonly FunctionCallNode NULL = new(TupleNode.NULL, ValueNode.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
