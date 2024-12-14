namespace Lotus;

[Flags]
public enum NumberKind
{
    Unknown = 0,
    Unsigned = 1 << 0,
    Int = 1 << 1,
    Long = 1 << 2,
    Float = 1 << 3,
    Double = 1 << 4,

    UInt = Unsigned | Int,
    ULong = Unsigned | Long
}

public static class NumberKindExtension
{
    public static string AsTypeName(this NumberKind kind)
        => kind switch {
            NumberKind.Int => "int",
            NumberKind.UInt => "uint",
            NumberKind.Long => "long",
            NumberKind.ULong => "ulong",
            NumberKind.Float => "float",
            NumberKind.Double => "double",
            _ => throw new ArgumentOutOfRangeException($"Number kind '{kind}' does not have a corresponding type name")
        };
}