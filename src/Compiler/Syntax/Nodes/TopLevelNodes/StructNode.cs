namespace Lotus.Syntax;

public sealed record StructNode(
    Token Token,
    TypeDecName Name,
    Tuple<StructField> Fields,
    ImmutableArray<Token> Modifiers
) : TopLevelNode(Token, new LocationRange(Token, Fields)){
    public new static readonly StructNode NULL = new(Token.NULL, TypeDecName.NULL, Tuple<StructField>.NULL, default) { IsValid = false };

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}

public sealed record StructField(
    IdentNode Name,
    NameNode Type,
    ValueNode DefaultValue,
    Token EqualSign
) : Parameter(Type, Name, new LocationRange(Name, DefaultValue == ValueNode.NULL ? Name : DefaultValue)) {
    public static readonly StructField NULL = new(
        IdentNode.NULL,
        NameNode.NULL,
        ValueNode.NULL,
        Token.NULL
    ) { IsValid = false };

    public bool HasDefaultValue => DefaultValue != ValueNode.NULL;
}