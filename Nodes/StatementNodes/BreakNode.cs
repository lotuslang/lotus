public class BreakNode : StatementNode
{
    public new static readonly BreakNode NULL = new(Token.NULL, false);

    public BreakNode(Token breakToken, bool isValid = true) : base(breakToken, breakToken.Location, isValid) { }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}