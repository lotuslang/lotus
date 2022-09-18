using System.Collections;
using System.Runtime.CompilerServices;

public sealed record Tuple<T> : ILocalized, IEnumerable<T>
{
    public T this[int i] => Items[i];

    public ImmutableArray<T> Items { get; init; }
    public Token OpeningToken { get; }
    public Token ClosingToken { get; }

    public static readonly Tuple<T> NULL = new(ImmutableArray<T>.Empty, Token.NULL, Token.NULL) { IsValid = false };

    public Tuple(ImmutableArray<T> items, Token opening, Token closing) {
        Items = items;
        OpeningToken = opening;
        ClosingToken = closing;
        Location = new LocationRange(opening, closing);
    }

    public LocationRange Location { get; init; }

    public bool IsValid { get; set; }

    public int Count => Items.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableArray<T>.Enumerator GetEnumerator() => Items.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)Items).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Items).GetEnumerator();
}