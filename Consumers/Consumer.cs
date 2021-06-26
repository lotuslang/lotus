using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public class Consumer<T> : IConsumer<T>
{
    private bool reconsumeFlag;

    private readonly Stack<T> inputStack;

    [property: AllowNull]
    public T Current { get; protected set; }

    public T Default { get; protected set; }

    // we keep it because it's cleaner and faster performance-wise than copying and storing
    // at each modification
    protected Location pos;

    public LocationRange Position => pos;

    protected Consumer() {
        inputStack = new Stack<T>();
        pos = new Location(1, -1);
        Default = default(T)!;
        Current = Default;
    }

    public Consumer(IEnumerable<T> enumerable, T defaultValue) : this() {
        Default = defaultValue;
        inputStack = new Stack<T>(enumerable.Reverse());
    }

    public Consumer(IConsumer<T> consumer) : this() {
        Default = consumer.Default;

        foreach (var item in consumer) {
            inputStack.Push(item);
        }

        inputStack = new Stack<T>(inputStack);
    }

    public bool Consume([MaybeNullWhen(false)] out T item) {
        if (inputStack.Count == 0) {
            Current = Default;
            item = Default;
            return false;
        }

        item = Consume()!;

        return true;
    }

    public T Consume() {

        if (reconsumeFlag) {
            reconsumeFlag = false;
            pos = pos with { column = pos.column + 1 };
            return Current;
        }

        if (inputStack.Count == 0) {
            Current = Default!;
            return Current;
        }

        pos = pos with { column = pos.column + 1 };

        Current = inputStack.Pop();

        return Current;
    }

    public void Reconsume() {
        reconsumeFlag = true;

        pos = pos with { column = pos.column - 1 };
    }


    public T Peek() {
        if (reconsumeFlag) return Current;

        if (!inputStack.TryPeek(out T? result)) result = Default;

        return result;
    }

    public Consumer<T> Clone() => new Consumer<T>(this);

    IConsumer<T> IConsumer<T>.Clone() => Clone();
}