using System.Collections;

public class ConsumerEnumerator<T> : IEnumerator<T>
{
    private readonly IConsumer<T> consumer;

    private T _current;

    public T Current => _current;

    object IEnumerator.Current => _current!; // i dont care tbh

    public ConsumerEnumerator(IConsumer<T> consumer) {
        this.consumer = consumer;
        _current = consumer.Default;
    }


    public bool MoveNext() {
        if (!consumer.Consume(out _current)) {
            //_current = consumer.Default;

            return false;
        }

        return true;
    }

    public void Reset() => throw new NotImplementedException();
    // suggested by roslyn, don't know either :shrug:
    public void Dispose() => GC.SuppressFinalize(this);
}