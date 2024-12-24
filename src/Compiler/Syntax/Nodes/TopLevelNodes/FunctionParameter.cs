namespace Lotus.Syntax;

/// <summary>You define parameters, you make arguments</summary>
public record FunctionParameter(
    NameNode Type,
    IdentNode Name,
    ValueNode? DefaultValue,
    Token? EqualSign
) : Parameter(Type, Name, new LocationRange(Type, DefaultValue ?? Name))
{
    public static readonly FunctionParameter NULL = new(
        NameNode.NULL,
        IdentNode.NULL
    ) { IsValid = false };

    public FunctionParameter(NameNode type, IdentNode name) : this(type, name, null, null) { }

    [MemberNotNullWhen(true, nameof(DefaultValue), nameof(EqualSign))]
    public bool HasDefaultValue => DefaultValue is not null;
}