using System.Runtime.CompilerServices;

namespace Lotus.Utils;

// non-nullable field must contain a null value when exiting constructor
#pragma warning disable CS8618

public readonly struct Union<T, U>
{
    private readonly T t;
    private readonly U u;
    private readonly int tag;

    public Union(T item) { t = item; tag = 0; }
    public Union(U item) { u = item; tag = 1; }

    public Union() => tag = -1;

    public TResult Match<TResult>(Func<T, TResult> f, Func<U, TResult> g) {
        switch (tag) {
            case 0: return f(t);
            case 1: return g(u);
            default:
                ThrowNoInit();
                return default!;
        }
    }

    public void Match(Action<T> f, Action<U> g) {
        switch (tag) {
            case 0: f(t); break;
            case 1: g(u); break;
            default: ThrowNoInit(); break;
        }
    }

    public bool Is<V>() {
        switch (tag) {
            case 0: return t is V;
            case 1: return u is V;
            default:
                ThrowNoInit();
                return false;
        }
    }

    public bool Is<V>([NotNullWhen(true)] out V? v) {
        v = default;

        switch (tag) {
            case 0:
                if (t is V tmp1) {
                    v = tmp1;
                    return true;
                } else {
                    return false;
                }
            case 1:
                if (u is V tmp2) {
                    v = tmp2;
                    return true;
                } else {
                    return false;
                }
            default:
                ThrowNoInit();
                return false;
        }
    }

    public void ThrowNoInit() => throw new InvalidOperationException("This union wasn't properly initialized.");

    public bool IsNull() => t is null && u is null;

    public static implicit operator Union<T, U>(T t) => new(t);
    public static implicit operator Union<T, U>(U u) => new(u);

    public static implicit operator Union<U, T>(Union<T, U> union)
        => union.Match<Union<U, T>>(f => f, g => g);

    public static explicit operator T(Union<T, U> union) => union.t;
    public static explicit operator U(Union<T, U> union) => union.u;

    public override string ToString()
        => Match(t => t?.ToString(), u => u?.ToString())!;
    public override int GetHashCode()
        => Match(t => t?.GetHashCode(), u => u?.GetHashCode()) ?? 0;
}

public readonly struct Union<T, U, V>
{
    private readonly T t;
    private readonly U u;
    private readonly V v;
    private readonly int tag;

    public Union(T item) { t = item; tag = 0; }
    public Union(U item) { u = item; tag = 1; }
    public Union(V item) { v = item; tag = 2; }

    [Obsolete("You should never use Union's parameterless constructor")]
    public Union() => tag = -1;

    public TResult Match<TResult>(Func<T, TResult> f, Func<U, TResult> g, Func<V, TResult> h) {
        switch (tag) {
            case 0: return f(t);
            case 1: return g(u);
            case 2: return h(v);
            default:
                ThrowNoInit();
                return default!;
        }
    }

    public void Match(Action<T> f, Action<U> g, Action<V> h) {
        switch (tag) {
            case 0:
                f(t);
                break;
            case 1:
                g(u);
                break;
            case 2:
                h(v);
                break;
            default:
                ThrowNoInit();
                break;
        }
    }

    public bool Is<W>() {
        switch (tag) {
            case 0: return t is W;
            case 1: return u is W;
            case 2: return v is W;
            default:
                ThrowNoInit();
                return false;
        }
    }

    public bool Is<W>([NotNullWhen(true)] out W? w) {
        w = default;

        switch (tag) {
            case 0:
                if (t is W tmp1) {
                    w = tmp1;
                    return true;
                } else {
                    return false;
                }
            case 1:
                if (u is W tmp2) {
                    w = tmp2;
                    return true;
                } else {
                    return false;
                }
            case 2:
                if (u is W tmp3) {
                    w = tmp3;
                    return true;
                } else {
                    return false;
                }
            default:
                ThrowNoInit();
                return false;
        }
    }

    public void ThrowNoInit() => throw new InvalidOperationException("This union wasn't properly initialized.");

    public bool IsNull() => t is null && u is null && v is null;

    public static implicit operator Union<T, U, V>(T t) => new(t);
    public static implicit operator Union<T, U, V>(U u) => new(u);
    public static implicit operator Union<T, U, V>(V v) => new(v);

    public static implicit operator Union<T, V, U>(Union<T, U, V> union)
        => union.Match<Union<T, V, U>>(f => f, g => g, h => h);
    public static implicit operator Union<V, T, U>(Union<T, U, V> union)
        => union.Match<Union<V, T, U>>(f => f, g => g, h => h);
    public static implicit operator Union<U, T, V>(Union<T, U, V> union)
        => union.Match<Union<U, T, V>>(f => f, g => g, h => h);
    public static implicit operator Union<U, V, T>(Union<T, U, V> union)
        => union.Match<Union<U, V, T>>(f => f, g => g, h => h);

    public static explicit operator T(Union<T, U, V> union) => union.t;
    public static explicit operator U(Union<T, U, V> union) => union.u;
    public static explicit operator V(Union<T, U, V> union) => union.v;

    public override string ToString()
        => Match(t => t?.ToString(), u => u?.ToString(), v => v?.ToString())!;
    public override int GetHashCode()
        => Match(t => t?.GetHashCode(), u => u?.GetHashCode(), v => v?.GetHashCode()) ?? 0;
}

public readonly struct None
{
    public static readonly None Instance = new();
}

public readonly struct Result<T>
{
    private readonly T t;

    public readonly T Value => t;

    private readonly bool isOk;

    public Result() {
        t = default!;
        isOk = false;
    }

    public Result(T value) {
        t = value;
        isOk = true;
    }

    private static readonly Result<T> _err = new();
    public static ref readonly Result<T> Error => ref _err;

    public void OnError(Action act) {
        if (!isOk) act();
    }

    public T Rescue(Func<T> factory)
        => isOk ? t : factory();

    public T Rescue(T val)
        => isOk ? t : val;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(Action<T> onOk, Action onError) {
        if (isOk)
            onOk(t!);
        else
            onError();
    }

    public TResult Match<TResult>(Func<T, TResult> onOk, Func<TResult> onError)
        => isOk ? onOk(t) : onError();

    [MemberNotNullWhen(true, nameof(Value), nameof(t))]
    public readonly bool IsOk() => isOk;

    public Union<T, None> AsUnion() => (Union<T, None>)this;

    public static implicit operator Result<T>(T t) => new(t);
    public static implicit operator Union<T, None>(Result<T> res)
        => res.isOk ? new(res.t) : new(None.Instance);

    public override string? ToString()
        => isOk ? t?.ToString() : "";
    public override int GetHashCode()
        => isOk ? (t?.GetHashCode() ?? 0) : 0;
}