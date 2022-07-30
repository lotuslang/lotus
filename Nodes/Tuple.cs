using System.Collections;
using System.Runtime.CompilerServices;

public sealed record Tuple<T> : ILocalized, IEnumerable<T>
{
    public T this[int i] => Items[i];

    public ImmutableArray<T> Items { get; init; }
    public Token OpeningToken { get; }
    public Token ClosingToken { get; }

    public static readonly Tuple<T> NULL = new(Array.Empty<T>(), Token.NULL, Token.NULL, false);

    public Tuple(IEnumerable<T> items, Token opening, Token closing, bool isValid = true) {
        Items = items.ToImmutableArray();
        OpeningToken = opening;
        ClosingToken = closing;
        IsValid = isValid;
        Location = new LocationRange(opening, closing);
    }

    public LocationRange Location { get; init; }

    public bool IsValid { get; set; }

    public int Count => Items.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableArray<T>.Enumerator GetEnumerator() => Items.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => (Items as IEnumerable<T>).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => (Items as IEnumerable).GetEnumerator();
}