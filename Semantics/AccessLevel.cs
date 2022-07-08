/// <summary>
/// Whether the member/type is public, private, protected, internal, or public.
/// </summary>
[Flags]
public enum AccessLevel {
	/// <summary>
	/// NOTE : This is only used internally to represent a accessor that does not exist
	/// </summary>
	Unreachable = -1,
	Private = 0,
	Protected = 1,
	Internal = 2,
	InternalProtected = 3,
	Public = 4
}