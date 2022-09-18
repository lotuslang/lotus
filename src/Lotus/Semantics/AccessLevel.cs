/// <summary>
/// Whether the member/type is public, private, protected, internal, or public.
/// </summary>
[Flags]
public enum AccessLevel {
	Default = 0,
	Private = 1,
	Internal = 2,
	Public = 8
}