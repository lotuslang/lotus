public interface IAccessible
{
    AccessLevel AccessLevel { get; set; }

    Token AccessToken { get; set; }

    AccessLevel ValidLevels { get; }
    AccessLevel DefaultAccessLevel { get; }
}