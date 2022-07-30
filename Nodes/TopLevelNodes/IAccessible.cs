public interface IAccessible
{
    AccessLevel AccessLevel { get; set; }

    AccessLevel ValidLevels { get; }
    AccessLevel DefaultAccessLevel { get; }
}