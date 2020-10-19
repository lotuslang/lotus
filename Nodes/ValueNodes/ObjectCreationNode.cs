public class ObjectCreationNode : ValueNode
{
    public FunctionCallNode InvocationNode { get; protected set; }

    public ValueNode TypeName => InvocationNode.FunctionName;

    public ObjectCreationNode(FunctionCallNode invoke, ComplexToken newToken, bool isValid = true) : base(newToken, isValid) {
        InvocationNode = invoke;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}