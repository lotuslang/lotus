namespace Lotus.Semantics;

public sealed class UnionTypeInfo(
    IEnumerable<TypeInfo> types,
    SemanticUnit unit
) : TypeInfo(unit)
{
    // todo: this should really be an ImmutableArray
    public List<TypeInfo> Types { get; } = [.. types];

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}