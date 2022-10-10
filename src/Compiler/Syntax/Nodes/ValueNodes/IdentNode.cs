namespace Lotus.Syntax;

public sealed record IdentNode(IdentToken Token) : NameNode(Token, ImmutableArray.Create(Token), Token.IsValid)
{
    public new static readonly IdentNode NULL = new(IdentToken.NULL);

    public new IdentToken Token { get => (base.Token as IdentToken)!; init => base.Token = value; }

    public string Value => Token.Representation;

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
