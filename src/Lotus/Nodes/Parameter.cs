public abstract record Parameter(NameNode Type, IdentNode Name) : ILocalized {
    public LocationRange Location { get; } = new(Type.Location, Name.Location);

    public bool IsValid { get; set; }

    public Parameter(NameNode type, IdentNode name, LocationRange loc)
        : this(type, name) => Location = loc;
}