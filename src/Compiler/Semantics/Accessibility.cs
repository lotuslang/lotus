namespace Lotus.Semantics;

[Flags]
public enum Accessibility {
    Default = 0,
    Private = 1,
    Protected = 2,
    Internal = 4,
    Public = 8,

    ProtectedInternal = Protected | Internal,
}