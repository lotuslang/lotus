using System.Diagnostics.CodeAnalysis;

public class Consumer<T> : IConsumer<T>
{
    private bool reconsumeFlag;

    private readonly Stack<T> inputStack;

    [property: AllowNull]
    public T Current { get; protected set; }

    public T Default { get; protected set; }

    protected Location internalPos;

    public LocationRange Position => (Current is ILocalized tLoc ? tLoc.Location : internalPos);

    protected Consumer() {
        inputStack = new Stack<T>();
        internalPos = new Location(1, -1);
        Default = default(T)!;
        Current = Default;
    }

    public Consumer(IEnumerable<T> enumerable, T defaultValue, string filename) : this() {
        Default = defaultValue;
        inputStack = new Stack<T>(enumerable.Reverse());
        internalPos = internalPos with { filename = filename };
    }

    public Consumer(Consumer<T> consumer) : this() {
        Default = consumer.Default;

        inputStack = consumer.inputStack.Clone();
        internalPos = internalPos with { filename = consumer.Position.filename };
    }

    public Consumer(IConsumer<T> consumer) : this() {
        Default = consumer.Default;

        foreach (var item in consumer) {
            inputStack.Push(item);
        }

        inputStack = new Stack<T>(inputStack);
        internalPos = internalPos with { filename = consumer.Position.filename };
    }

    public bool Consume(out T item) {
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
            internalPos = internalPos with { column = internalPos.column + 1 };
            return Current;
        }

        if (inputStack.Count == 0) {
            Current = Default!;
            return Current;
        }

        internalPos = internalPos with { column = internalPos.column + 1 };

        Current = inputStack.Pop();

        return Current;
    }

    public void Reconsume() {
        reconsumeFlag = true;

        internalPos = internalPos with { column = internalPos.column - 1 };
    }


    public T Peek() {
        if (reconsumeFlag) return Current;

        if (!inputStack.TryPeek(out var result)) result = Default;

        return result;
    }

    public Consumer<T> Clone() => new(this);

    IConsumer<T> IConsumer<T>.Clone() => Clone();

    internal class Enumerator : IEnumerator<T> {
        private IConsumer<T> consumer;
        private readonly IConsumer<T> originalConsumer;

        private T _current;

        public T Current => _current;

        object System.Collections.IEnumerator.Current => _current!; // i dont care tbh

        public Enumerator(IConsumer<T> consumer) {
            this.consumer = consumer;
            _current = consumer.Default;
            originalConsumer = consumer.Clone();
        }


        public bool MoveNext() {
            if (!consumer.Consume(out _current)) {
                //_current = consumer.Default;

                return false;
            }

            return true;
        }

        public void Reset() => consumer = originalConsumer.Clone();
        // suggested by roslyn, don't know either :shrug:
        public void Dispose() => GC.SuppressFinalize(this);
    }
}