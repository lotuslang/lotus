namespace Lotus.Utils;

// The BCL's HashCode class isn't deterministic across AppDomain (e.g. across runs of the same app)
public struct DeterministicHashCode
{
    private int current;

    public readonly int ToHashCode() => current;

    private static readonly DeterministicStringComparer stringComparer = DeterministicStringComparer.Instance;

    public void Add(int hash)
        => current = (current * 21) + hash;

    public void Add<T1>(T1 value) where T1 : notnull
        => Add(value.GetHashCode());
    public void Add<T1>(T1 value, IEqualityComparer<T1> eq) where T1 : notnull
        => Add(eq.GetHashCode(value));

    public static int Combine(int hash1, int hash2)
        => ((hash1 << 5) + hash1) ^ hash2;

    public static int Combine<T1>(T1 t1, string s) where T1 : notnull
        => (t1.GetHashCode() * 21) + stringComparer.GetHashCode(s);
    public static int Combine<T1>(string s, T1 t1) where T1 : notnull
        => (stringComparer.GetHashCode(s) * 21) + t1!.GetHashCode();
    public static int Combine<T1, T2>(T1 t1, T2 t2) where T1 : notnull where T2 : notnull
        => Combine(t1.GetHashCode(), t2.GetHashCode());
}