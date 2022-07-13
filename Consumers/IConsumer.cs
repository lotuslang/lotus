using System.Collections;

public interface IConsumer<T> : IEnumerable<T>
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
    //[return: MaybeNull]
    ref readonly T Consume();

    /// <summary>
    /// Consumes an object and returns it.
    /// </summary>
    /// <param name="success">True if the operation succeeded, false otherwise</param>
    /// <returns></returns>
    bool Consume(out T item);

    T Peek();

    IConsumer<T> Clone();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Consumer<T>.Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}