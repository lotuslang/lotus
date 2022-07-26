public record IfNode(
    ParenthesizedValueNode Condition,
    Tuple<StatementNode> Body,
    ElseNode ElseNode,
    Token Token,
    bool IsValid = true
) : StatementNode(Token, new LocationRange(Token.Location, ElseNode != ElseNode.NULL ? ElseNode.Location : Body.Location), IsValid)
{
    public new static readonly IfNode NULL = new(ParenthesizedValueNode.NULL, Tuple<StatementNode>.NULL, ElseNode.NULL, Token.NULL, false);

    public bool HasElse => ElseNode != ElseNode.NULL;

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}