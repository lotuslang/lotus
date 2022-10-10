namespace Lotus.Syntax;

public sealed record TypeDecName(IdentNode TypeName, NameNode Parent, Token ColonToken)
{
    public bool HasParent => Parent != NameNode.NULL;

    public bool IsValid { get; set; }

    public static readonly TypeDecName NULL = new (IdentNode.NULL, NameNode.NULL, Token.NULL) { IsValid = false };

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public T Accept<T>(Visitors.ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}