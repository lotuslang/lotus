using System.Runtime.CompilerServices;

namespace Lotus.Utils;

public sealed class Union<T, U>
{
    private readonly T? t;
    private readonly U? u;
    private readonly int tag;

    public Union(T item) { t = item; tag = 0; }
    public Union(U item) { u = item; tag = 1; }

    public TResult Match<TResult>(Func<T, TResult> f, Func<U, TResult> g) {
        switch (tag) {
            case 0: return f(t!);
            case 1: return g(u!);
            default: throw new Exception("Unrecognized tag value: " + tag);
        }
    }

    public void Match(Action<T> f, Action<U> g) {
        switch (tag) {
            case 0: f(t!); break;
            case 1: g(u!); break;
            default: throw new Exception("Unrecognized tag value: " + tag);
        }
    }

    public bool Is<V>() {
        switch (tag) {
            case 0: return t is V;
            case 1: return u is V;
            default: throw new Exception("Unrecognized tag value: " + tag);
        }
    }

    public bool IsNull() => (t is null) && (u is null);

    public static implicit operator Union<T, U>(T t) => new(t);
    public static implicit operator Union<T, U>(U u) => new(u);

    public static explicit operator T(Union<T, U> union) => union.t!;
    public static explicit operator U(Union<T, U> union) => union.u!;

    public override string ToString()
        => Match(t => t?.ToString(), u => u?.ToString())!;
}

public class Union<T, U, V>
{
    private readonly T? t;
    private readonly U? u;
    private readonly V? v;
    private readonly int tag;

    public Union(T item) { t = item; tag = 0; }
    public Union(U item) { u = item; tag = 1; }
    public Union(V item) { v = item; tag = 2; }

    public TResult Match<TResult>(Func<T, TResult> f, Func<U, TResult> g, Func<V, TResult> h) {
        switch (tag) {
            case 0: return f(t!);
            case 1: return g(u!);
            case 2: return h(v!);
            default: throw new Exception("Unrecognized tag value: " + tag);
        }
    }

    public void Match(Action<T> f, Action<U> g, Action<V> h) {
        switch (tag) {
            case 0:
                f(t!);
                break;
            case 1:
                g(u!);
                break;
            case 2:
                h(v!);
                break;
            default:
                throw new Exception("Unrecognized tag value: " + tag);
        }
    }

    public bool IsNull() => t is null && u is null && v is null;

    public static implicit operator Union<T, U, V>(T t) => new(t);
    public static implicit operator Union<T, U, V>(U u) => new(u);
    public static implicit operator Union<T, U, V>(V v) => new(v);

    public override string ToString()
        => Match(t => t?.ToString(), u => u?.ToString(), v => v?.ToString())!;
}

public sealed class None {
    public static readonly None Instance = new();
}

public sealed class Result<T>
{
    private readonly T? t;

    public ref readonly T? Value => ref t;

    private readonly bool isOk = false;

    private Result() { }

    public Result(T value) {
        t = value;
        isOk = true;
    }

    private static readonly Result<T> _err = new();
    public static ref readonly Result<T> Error => ref _err;

    public void OnError(Action act) {
        if (!isOk) act();
    }

    public T Rescue(Func<T> gen)
        => isOk ? t! : gen();

    public T Rescue(T val)
        => isOk ? t! : val;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(Action<T> f, Action g) {
        if (isOk)
            f(t!);
        else
            g();
    }

    public TResult Match<TResult>(Func<T, TResult> f, Func<TResult> g)
        => isOk ? f(t!) : g();

    [MemberNotNullWhen(true, nameof(Value))]
    public ref readonly bool IsOk() => ref isOk;

    public Union<T, None> AsUnion() => (Union<T, None>)this;

    public static implicit operator Result<T>(T t) => new(t);
    public static implicit operator Union<T, None>(Result<T> res)
        => res.isOk ? new(res.t!) : new(None.Instance);
}