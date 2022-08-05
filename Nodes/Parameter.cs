public abstract record Parameter(NameNode Type, IdentNode Name) : ILocalized {
    private LocationRange _loc = new LocationRange(Type.Location, Name.Location);
    public LocationRange Location => _loc;

    public bool IsValid { get; set; }

    public Parameter(NameNode type, IdentNode name, LocationRange loc)
        : this(type, name) => _loc = loc;
}