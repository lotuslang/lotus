namespace Lotus.Syntax;

public sealed record TopLevelStatementNode(StatementNode Statement)
: TopLevelNode(Statement.Token, Statement.Location, Statement.IsValid)
{
    public static implicit operator StatementNode(TopLevelStatementNode node) => node.Statement;

    public new LocationRange Location {
        get => Statement.Location;
        init => Statement = Statement with { Location = value };
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}