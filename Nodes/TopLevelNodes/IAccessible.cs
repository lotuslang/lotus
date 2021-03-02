public interface IAccessible
{
    AccessLevel GetAccessLevel();

    Token AccessKeyword { get; set; }
}