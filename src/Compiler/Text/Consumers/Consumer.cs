namespace Lotus.Text;

public class Consumer<T> : IConsumer<T>
{
    protected ImmutableArray<T> _data;
    protected int _currIdx;

    public virtual ref readonly T Current {
        get {
            if (_currIdx < 0 || Unconsumed < 0)
                return ref ConstantDefault;
            else
                return ref _data.ItemRef(_currIdx);
        }
    }

    public int Unconsumed => _data.Length - (_currIdx + 1);

    public T ConstantDefault;

    public T Default => ConstantDefault;

    protected Location internalPos;

    public virtual LocationRange Position => Current is ILocalized tLoc ? tLoc.Location : internalPos;

    protected Consumer() {
        _data = ImmutableArray<T>.Empty;
        internalPos = new Location(1, 0);
        ConstantDefault = default(T)!;
        _currIdx = -1;
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
        if (Unconsumed <= 0) {
            _currIdx++; // increment it for Reconsume()
            return ref ConstantDefault;
        }

        _currIdx++;
        internalPos = internalPos with { column = internalPos.column + 1 };
        return ref _data.ItemRef(_currIdx);
    }

    public virtual void Reconsume()
        => _currIdx--;

    public virtual T Peek()
        => Unconsumed <= 0 ? ConstantDefault : _data[_currIdx + 1];

    public virtual Consumer<T> Clone() => new(this);

    IConsumer<T> IConsumer<T>.Clone() => Clone();
}