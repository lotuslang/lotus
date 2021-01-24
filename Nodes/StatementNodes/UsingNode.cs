public class UsingNode : StatementNode
{
    public new static readonly UsingNode NULL = new UsingNode(ComplexToken.NULL, ValueNode.NULL, false);

    public ValueNode ImportName { get; }

    public UsingNode(ComplexToken usingToken, ValueNode importName, bool isValid = true)
        : base(usingToken, new LocationRange(usingToken.Location, importName.Location), isValid)
    {
        ImportName = importName;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}