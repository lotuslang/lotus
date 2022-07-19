public abstract record Parameter(ValueNode Type, IdentNode Name, bool IsValid = true) : ILocalized {
    private LocationRange _loc = new LocationRange(Type.Location, Name.Location);
    public LocationRange Location => _loc;
}

public record ParameterList<TParam>(
    IList<TParam> Items,
    Token OpeningToken,
    Token ClosingToken,
    bool IsValid = true
) : TupleNode<TParam>(Items, OpeningToken, ClosingToken, IsValid) where TParam : Parameter {
    public new static readonly ParameterList<TParam> NULL
        = new ParameterList<TParam>(
            Array.Empty<TParam>(),
            Token.NULL,
            Token.NULL,
            false
        );

    public ParameterList(TupleNode<TParam> tuple)
        : this(tuple.Items, tuple.OpeningToken, tuple.ClosingToken, tuple.IsValid) {}
}