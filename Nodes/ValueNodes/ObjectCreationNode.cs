public class ObjectCreationNode : ValueNode
{
    public new static readonly ObjectCreationNode NULL = new ObjectCreationNode(FunctionCallNode.NULL, ComplexToken.NULL, false);

    public FunctionCallNode InvocationNode { get; protected set; }

    public new ComplexToken Token { get; protected set; }

    public ValueNode TypeName => InvocationNode.FunctionName;

    public ObjectCreationNode(FunctionCallNode invoke, ComplexToken newToken, bool isValid = true)
        : base(newToken, new LocationRange(newToken.Location, invoke.Location), isValid)
    {
        InvocationNode = invoke;
        Token = newToken;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}