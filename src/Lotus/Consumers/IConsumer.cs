namespace Lotus;

public interface IConsumer<T>
{
    /// <summary>
    /// Gets the last object consumed by this IConsumer.
    /// </summary>
    /// <value>Last object consumed.</value>
    ref readonly T Current { get; }

    T Default { get; }

    LocationRange Position { get; }

    /// <summary>
    /// Reconsumes the last object, so that the next time the Consume() method is called,
    /// the last object will be consumed instead.
    /// </summary>
    void Reconsume();

    /// <summary>
    /// Consumes an object and returns it.
    /// </summary>
    /// <returns>The consumed object</returns>
    ref readonly T Consume();

    /// <summary>
    /// Tries to consume a value and returns <see cref="IConsumer{T}.Default"/> if none was available
    /// </summary>
    /// <param name="item">The value that was consumed, or <see cref="IConsumer{T}.Default"/> when not possible</param>
    /// <returns>True if a value was available, false otherwise</returns>
    bool Consume(out T item);

    T Peek();

    IConsumer<T> Clone();
}