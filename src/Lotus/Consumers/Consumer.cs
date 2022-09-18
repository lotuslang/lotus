using Lotus.Error;

namespace Lotus.Text;

public class Consumer<T> : IConsumer<T>
{
    protected bool _atStart;

    protected ImmutableArray<T> _data;
    protected int _currIdx;

    public virtual ref readonly T Current => ref _data.ItemRef(_currIdx);

    public int Count => _data.Length - (_currIdx + 1);

    public T ConstantDefault;

    public T Default => ConstantDefault;

    protected Location internalPos;

    public virtual LocationRange Position => Current is ILocalized tLoc ? tLoc.Location : internalPos;

    protected Consumer() {
        _data = ImmutableArray<T>.Empty;
        _atStart = true;
        internalPos = new Location(1, 0);
        ConstantDefault = default(T)!;
    }

    public Consumer(ImmutableArray<T> enumerable, T defaultValue, string filename) : this() {
        ConstantDefault = defaultValue;

        _data = enumerable;

        internalPos = internalPos with { filename = filename };
    }

    public Consumer(Consumer<T> consumer) : this() {
        ConstantDefault = consumer.Default;

        _data = consumer._data;
        _currIdx = consumer._currIdx;
        _atStart = consumer._atStart;

        internalPos = consumer.internalPos;
    }

    public Consumer(IConsumer<T> consumer) : this() {
        ConstantDefault = consumer.Default;

        _data = consumer.ToImmutableArray();

        internalPos = internalPos with { filename = consumer.Position.filename };
    }

    public virtual bool Consume(out T item) {
        item = Consume();

        return !EqualityComparer<T>.Default.Equals(item, Default);
    }

    public virtual ref readonly T Consume() {
        if (!_atStart && Count <= 0) {
            _currIdx++; // still update because reconsume() might be called
            return ref ConstantDefault;
        }

        if (!_atStart) {
            _currIdx++;
        } else {
            _atStart = false;
        }

        internalPos = internalPos with { column = internalPos.column + 1 };

        return ref _data.ItemRef(_currIdx);
    }

    public virtual void Reconsume() {
        if (_currIdx <= 0)
            _atStart = true;
        else
            _currIdx--;
    }

    public virtual T Peek()
        => Count <= 0 ? ConstantDefault : _data[_atStart ? _currIdx : _currIdx + 1];

    public virtual Consumer<T> Clone() => new(this);

    IConsumer<T> IConsumer<T>.Clone() => Clone();
}