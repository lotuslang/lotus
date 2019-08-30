using System;
using System.Linq;
using System.Collections.Generic;

public class Consumer<T> : IConsumer<T>
{
    private bool reconsumeFlag;

    private Stack<T> stack;

    protected T current;

    public T Current {
        get => current;
    }

    public Location pos;

    public Location Position {
        get => pos;
    }

    public Consumer(IEnumerable<T> enumerable) {
        stack = new Stack<T>(enumerable.Reverse());
        pos = new Location(0, 0);
    }

    public Consumer(IConsumer<T> consumer) {
        var stack = new Stack<T>();

        while (consumer.Consume(out T item)) {
            stack.Push(item);
        }

        this.stack = new Stack<T>(stack);

        pos = new Location(0, 0);
    }

    public bool Consume(out T item) {
        if (stack.Count == 0) {
            current = item = default(T);
            return false;
        }

        item = Consume();
        return true;
    }

    public T Consume() {
        if (stack.Count == 0) {
            current = default(T);
            return current;
        }

        pos.column++;

        current = stack.Pop();

        return current;
    }

    public void Reconsume() {
        reconsumeFlag = true;

        pos.column--;
    }

    public T Peek() {
        if (reconsumeFlag) return current;

        stack.TryPeek(out T result);

        return result;
    }
}