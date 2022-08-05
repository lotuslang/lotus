public sealed record FullNameNode(OperatorToken DotToken, ImmutableArray<IdentToken> Parts)
: NameNode(DotToken, Parts)
{

    public FullNameNode(NameNode left, NameNode right, OperatorToken dotToken)
        : this(dotToken, left.Parts.Concat(right.Parts).ToImmutableArray()) {
        IsValid = left.IsValid && right.IsValid;
    }
}