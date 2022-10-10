namespace Lotus.Utils;

public struct DeterministicHashCode
{
    private int current;

    private static readonly DeterministicStringComparer stringComparer = DeterministicStringComparer.Instance;

    public void Add<T1>(T1 value) => current = (current * 21) + value!.GetHashCode();

    public void Add<T1>(T1 value, IEqualityComparer<T1> eq) => current = (current * 21) + eq.GetHashCode(value!);

    public static int Combine<T1>(T1 t1, string s) => (t1!.GetHashCode() * 21) + stringComparer.GetHashCode(s);

    public static int Combine<T1>(string s, T1 t1) => (stringComparer.GetHashCode(s) * 21) + t1!.GetHashCode();

    public static int Combine<T1, T2>(T1 t1, T2 t2) => (t1!.GetHashCode() * 21) + t2!.GetHashCode();

    public int ToHashCode() => current;
}