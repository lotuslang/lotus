using System.Diagnostics.CodeAnalysis;

public interface IConsumer<T>
{
    /// <summary>
    /// Gets the last object consumed by this IConsumer.
    /// </summary>
    /// <value>Last object consumed.</value>
    T Current { get; }

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
    T Consume();

    /// <summary>
    /// Consumes an object and returns it.
    /// </summary>
    /// <param name="success">True if the operation succeeded, false otherwise</param>
    /// <returns></returns>
    bool Consume([MaybeNullWhen(false)] out T item);

    T Peek();

    //T[] Peek(int n);
}