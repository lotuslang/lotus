public class UsingNode : TopLevelNode
{
    public new static readonly UsingNode NULL = new(Token.NULL, ValueNode.NULL, false);

    public ValueNode ImportName { get; }

    public UsingNode(Token usingToken, ValueNode importName, bool isValid = true)
        : base(usingToken, new LocationRange(usingToken.Location, importName.Location), isValid)
    {
        ImportName = importName;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}