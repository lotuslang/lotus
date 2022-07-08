public record TypeDecName(IdentNode TypeName, ValueNode Parent, Token ColonToken, bool isValid) {

    public static readonly TypeDecName NULL = new (IdentNode.NULL, ValueNode.NULL, Token.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}