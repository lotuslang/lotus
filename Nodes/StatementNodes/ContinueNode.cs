public class ContinueNode : StatementNode
{
    public new static readonly ContinueNode NULL = new ContinueNode(ComplexToken.NULL, false);

    public ContinueNode(ComplexToken continueToken, bool isValid = true) : base(continueToken, continueToken.Location, isValid) { }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(StatementVisitor<T> visitor) => visitor.Visit(this);
}