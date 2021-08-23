public class IfNode : StatementNode
{
    public new static readonly IfNode NULL = new(ParenthesizedValueNode.NULL, SimpleBlock.NULL, Token.NULL, false);

    public ParenthesizedValueNode Condition { get; }

    public SimpleBlock Body { get; }

    public ElseNode? ElseNode { get; }

    public bool HasElse { get => ElseNode != null; }

    public IfNode(ParenthesizedValueNode condition, SimpleBlock body, Token ifToken, bool isValid = true)
        : base(ifToken, new LocationRange(ifToken.Location, body.Location), isValid)
    {
        Condition = condition;
        Body = body;
        ElseNode = null;
    }

    public IfNode(ParenthesizedValueNode condition, SimpleBlock body, ElseNode elseNode, Token ifToken, bool isValid = true)
        : this(condition, body, ifToken, isValid)
    {
        ElseNode = elseNode;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}