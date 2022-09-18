public interface IValued<out TValue>
{
    TValue Value { get; }
}