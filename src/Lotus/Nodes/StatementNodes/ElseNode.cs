namespace Lotus.Syntax;

public sealed record ElseNode(Union<Tuple<StatementNode>, IfNode> BlockOrIfNode, Token Token)
: StatementNode(Token, new LocationRange(Token.Location, BlockOrIfNode.Match(b => b.Location, n => n.Location)))
{
    public new static readonly ElseNode NULL = new(Tuple<StatementNode>.NULL, Token.NULL) { IsValid = false };

    public Tuple<StatementNode> Body => BlockOrIfNode.Match(b => b, n => n.Body);

    public bool HasIf => BlockOrIfNode.Match(b => false, n => true);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}