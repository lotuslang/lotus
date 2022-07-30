public abstract record Parameter(NameNode Type, IdentNode Name, bool IsValid = true) : ILocalized {
    private LocationRange _loc = new LocationRange(Type.Location, Name.Location);
    public LocationRange Location => _loc;
}