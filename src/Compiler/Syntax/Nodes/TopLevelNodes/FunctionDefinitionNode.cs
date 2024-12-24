namespace Lotus.Syntax;

public sealed record FunctionDefinitionNode(
    FunctionHeaderNode Header,
    Tuple<StatementNode> Body
) : TopLevelNode(Header.Token, new LocationRange(Header, Body))
{
    public new static readonly FunctionDefinitionNode NULL
        = new(FunctionHeaderNode.NULL, Tuple<StatementNode>.NULL) { IsValid = false };

    public ImmutableArray<Token> Modifiers { get; init; }

    internal bool isInternal = false;

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}