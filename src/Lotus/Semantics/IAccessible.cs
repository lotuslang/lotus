namespace Lotus.Semantic;

public interface IAccessible
{
    AccessLevel AccessLevel { get; set; }

    Lotus.Syntax.Token AccessToken { get; set; }

    AccessLevel ValidLevels { get; }
    AccessLevel DefaultAccessLevel { get; }
}