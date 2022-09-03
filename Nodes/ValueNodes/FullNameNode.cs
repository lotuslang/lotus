public sealed record FullNameNode(OperatorToken DotToken, ImmutableArray<IdentToken> Parts)
: NameNode(DotToken, Parts)
{

    public FullNameNode(NameNode left, NameNode right, OperatorToken dotToken)
        : this(dotToken, left.Parts.AddRange(right.Parts)) {
        IsValid = left.IsValid && right.IsValid;
    }
}