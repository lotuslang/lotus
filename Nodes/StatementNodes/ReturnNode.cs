public class ReturnNode : StatementNode
{
    public new static readonly ReturnNode NULL = new(ValueNode.NULL, Token.NULL, false);

    public ValueNode Value { get; protected set; }

    public bool IsReturningValue => Value != ValueNode.NULL;

    public ReturnNode(ValueNode value, Token returnToken, bool isValid = true)
        : base(returnToken, new LocationRange(returnToken.Location, value.Location), isValid)
    {
        Value = value;
    }

    public ReturnNode(Token returnToken, bool isValid = true)
        : base(returnToken, returnToken.Location, isValid)
    {
        Value = ValueNode.NULL;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}
