public class Union<T, U>
{
    readonly T? t;
    readonly U? u;
    readonly int tag;

    public Union(T item) { t = item; tag = 0; }
    public Union(U item) { u = item; tag = 1; }

    public TResult Match<TResult>(Func<T, TResult> f, Func<U, TResult> g) {
        switch (tag) {
            case 0: return f(t!);
            case 1: return g(u!);
            default: throw new Exception("Unrecognized tag value: " + tag);
        }
    }

    public static implicit operator Union<T, U>(T t) => new(t);
    public static implicit operator Union<T, U>(U u) => new(u);

    public static explicit operator T(Union<T, U> union) => union.t!;
    public static explicit operator U(Union<T, U> union) => union.u!;



    public override string ToString()
        => Match(t => t!.ToString(), u => u!.ToString())!;
}

public class Union<T, U, V>
{
    readonly T? Item1;
    readonly U? Item2;
    readonly V? Item3;
    readonly int tag;

    public Union(T item) { Item1 = item; tag = 0; }
    public Union(U item) { Item2 = item; tag = 1; }
    public Union(V item) { Item3 = item; tag = 2; }

    public TResult Match<TResult>(Func<T, TResult> f, Func<U, TResult> g, Func<V, TResult> h) {
        switch (tag) {
            case 0: return f(Item1!);
            case 1: return g(Item2!);
            case 2: return h(Item3!);
            default: throw new Exception("Unrecognized tag value: " + tag);
        }
    }

    public void Match<TResult>(Action<T> f, Action<U> g, Action<V> h) {
        switch (tag) {
            case 0:
                f(Item1!);
                break;
            case 1:
                g(Item2!);
                break;
            case 2:
                h(Item3!);
                break;
            default:
                throw new Exception("Unrecognized tag value: " + tag);
        }
    }

    public static implicit operator Union<T, U, V>(T t) => new(t);
    public static implicit operator Union<T, U, V>(U u) => new(u);
    public static implicit operator Union<T, U, V>(V v) => new(v);

    public override string ToString()
        => Match(t => t!.ToString(), u => u!.ToString(), v => v!.ToString())!;
}

public sealed class None { }

public sealed class Result<T> : Union<T, None>
{
    public Result(T value) : base(value) { }

    public static implicit operator Result<T>(T t) => new(t);
}