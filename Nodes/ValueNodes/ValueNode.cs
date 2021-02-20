[System.Diagnostics.DebuggerDisplay("{Representation}")]
public class ValueNode : StatementNode
{
    /// <summary>
    /// This constant is the equivalent of "null". When a function doesn't return, it will actually set the `#return` variable to this constant.
    /// Variables that are assigned to a non-returning functions will actually be assigned this value.
    /// </summary>
    public static new readonly ValueNode NULL = new ValueNode("", Token.NULL, LocationRange.NULL, false);

    public ValueNode(Token token, LocationRange range, bool isValid = true) : this(token.Representation, token, range, isValid)
    { }

    public ValueNode(string rep, Token token, LocationRange range, bool isValid = true) : base(rep, token, range, isValid)
    { }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(StatementVisitor<T> visitor) => visitor.Visit(this);
}