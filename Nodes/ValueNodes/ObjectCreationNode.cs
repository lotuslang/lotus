public class ObjectCreationNode : ValueNode
{
    public new static readonly ObjectCreationNode NULL = new(FunctionCallNode.NULL, Token.NULL, false);

    public FunctionCallNode InvocationNode { get; protected set; }

    public new Token Token { get; protected set; }

    public ValueNode TypeName => InvocationNode.FunctionName;

    public ObjectCreationNode(FunctionCallNode invoke, Token newToken, bool isValid = true)
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