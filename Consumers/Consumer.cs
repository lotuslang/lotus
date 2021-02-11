using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable disable
public class Consumer<T> : IConsumer<T>
{
    private bool reconsumeFlag;

    private readonly Stack<T> inputStack;

    [property: AllowNull]
    public T Current { get; protected set; }

    public T Default => default(T);

    // we keep it because it's cleaner and faster performance-wise than copying and storing
    // at each modification
    protected Location pos;

    public LocationRange Position => pos;

    protected Consumer() {
        inputStack = new Stack<T>();
        pos = new Location(1, -1);
        Current = default(T)!;
    }

    public Consumer(IEnumerable<T> enumerable) : this() {
        inputStack = new Stack<T>(enumerable.Reverse());
    }

    public Consumer(IConsumer<T> consumer) : this() {
        while (consumer.Consume(out T item)) {
            inputStack.Push(item);
        }

        inputStack = new Stack<T>(inputStack);
    }

    public bool Consume([MaybeNullWhen(false)] out T item) {
        if (inputStack.Count == 0) {
            Current = default(T)!;
            item = default(T)!;
            return false;
        }

        item = Consume()!;

        return true;
    }

    [return: MaybeNull]
    public T Consume() {

        if (reconsumeFlag) {
            reconsumeFlag = false;
            pos = pos with { column = pos.column + 1 };
            return Current;
        }

        if (inputStack.Count == 0) {
            Current = default(T)!;
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

    [return: MaybeNull]
    public T Peek() {
        if (reconsumeFlag) return Current;

        inputStack.TryPeek(out T result);

        return result;
    }
}
#nullable restore