public sealed record TypeDecName(IdentNode TypeName, NameNode Parent, Token ColonToken, bool IsValid = true)
{
    private bool _hasParent = Parent != NameNode.NULL;
    public bool HasParent => _hasParent;

    public static readonly TypeDecName NULL = new (IdentNode.NULL, NameNode.NULL, Token.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}