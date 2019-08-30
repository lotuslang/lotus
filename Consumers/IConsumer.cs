using System;
using System.Collections;
using System.Collections.Generic;

public interface IConsumer<T>
{
    /// <summary>
    /// Gets the last object consumed by this IConsumer.
    /// </summary>
    /// <value>Last object consumed.</value>
    T Current { get; }

    Location Position { get; }

    /// <summary>
    /// Reconsumes the last object, so that the next time the Consume() method is called,
    /// the last object will be consumed instead.
    /// </summary>
    void Reconsume();

    /// <summary>
    /// Consumes an object and returns it.
    /// </summary>
    /// <returns>The consumed object</returns>
    T Consume();

    /// <summary>
    /// Consumes an object and returns it.
    /// </summary>
    /// <param name="success">True if the operation succeeded, false otherwise</param>
    /// <returns></returns>
    bool Consume(out T item);

    T Peek();
}