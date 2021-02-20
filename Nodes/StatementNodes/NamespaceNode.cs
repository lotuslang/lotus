public class NamespaceNode : StatementNode
{
    public new static readonly NamespaceNode NULL = new NamespaceNode(ValueNode.NULL, ComplexToken.NULL, false);

    public ValueNode NamespaceName { get; protected set; }

    public NamespaceNode(ValueNode namespaceName, ComplexToken namespaceToken, bool isValid = true)
        : base(namespaceToken, new LocationRange(namespaceToken.Location, namespaceName.Location), isValid)
    {
        NamespaceName = namespaceName;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(StatementVisitor<T> visitor) => visitor.Visit(this);
}
