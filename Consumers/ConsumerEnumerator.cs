using System.Collections;
using System.Collections.Generic;

public class ConsumerEnumerator<T> : IEnumerator<T>
{
    private IConsumer<T> consumer;

    private T _current;

    public T Current => _current;

    object IEnumerator.Current => throw new System.NotImplementedException();

    public ConsumerEnumerator(IConsumer<T> consumer) {
        this.consumer = consumer;
        _current = consumer.Default;
    }


    public bool MoveNext() {
        if (!consumer.Consume(out _current!)) {
            _current = consumer.Default;

            return false;
        }

        return true;
    }

    public void Reset() => throw new System.NotImplementedException();
    public void Dispose() { }
}