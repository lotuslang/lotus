public record ElseNode(Union<SimpleBlock, IfNode> BlockOrIfNode, Token Token, bool IsValid = true)
: StatementNode(Token, new LocationRange(Token.Location, BlockOrIfNode.Match(b => b.Location, n => n.Location)), IsValid)
{
    public new static readonly ElseNode NULL = new(SimpleBlock.NULL, Token.NULL, false);

    public SimpleBlock Body => BlockOrIfNode.Match(b => b, n => n.Body);

    public bool HasIf => BlockOrIfNode.Match(b => false, n => true);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}