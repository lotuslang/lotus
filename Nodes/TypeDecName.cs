public sealed record TypeDecName(IdentNode TypeName, NameNode Parent, Token ColonToken)
{
    private bool _hasParent = Parent != NameNode.NULL;
    public bool HasParent => _hasParent;

    public bool IsValid { get; set; }

    public static readonly TypeDecName NULL = new (IdentNode.NULL, NameNode.NULL, Token.NULL) { IsValid = false };

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}