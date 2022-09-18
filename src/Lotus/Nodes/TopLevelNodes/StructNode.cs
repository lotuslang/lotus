using Lotus.Semantic;

namespace Lotus.Syntax;

public sealed record StructNode(
    Token Token,
    TypeDecName Name,
    Tuple<StructField> Fields
) : TopLevelNode(Token, new LocationRange(Token, Fields)), IAccessible {
    public new static readonly StructNode NULL = new(Token.NULL, TypeDecName.NULL, Tuple<StructField>.NULL) { IsValid = false };

    public AccessLevel AccessLevel { get; set; } = AccessLevel.Public;

    public Token AccessToken { get; set; } = Token.NULL;

    AccessLevel IAccessible.DefaultAccessLevel => AccessLevel.Public;
    AccessLevel IAccessible.ValidLevels => AccessLevel.Public | AccessLevel.Internal;

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
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