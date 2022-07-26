public record Tuple<T> : ILocalized, IEnumerable<T>
{
    public T this[int i] => Items[i];

    public IList<T> Items { get; }
    public Token OpeningToken { get; }
    public Token ClosingToken { get; }

    public static readonly Tuple<T> NULL = new(Array.Empty<T>(), Token.NULL, Token.NULL, false);

    public Tuple(IList<T> items, Token opening, Token closing, bool isValid = true) {
        Items = items;
        OpeningToken = opening;
        ClosingToken = closing;
        IsValid = isValid;
        Location = new LocationRange(opening, closing);
    }

    public LocationRange Location { get; init; }

    public bool IsValid { get; set; }

    public int Count => Items.Count;

    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}