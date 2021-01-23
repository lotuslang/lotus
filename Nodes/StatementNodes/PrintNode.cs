public class PrintNode : StatementNode
{
    public ValueNode Value { get; }

    public PrintNode(ComplexToken printToken, ValueNode node) : base(printToken, printToken.Location) {
        Value = node;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}