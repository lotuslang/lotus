public abstract record Parameter(ValueNode Type, IdentNode Name, bool IsValid = true) : ILocalized {
    private LocationRange _loc = new LocationRange(Type.Location, Name.Location);
    public LocationRange Location => _loc;
}