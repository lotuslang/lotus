public sealed record FullNameNode(OperatorToken DotToken, ImmutableArray<IdentToken> Parts, bool IsValid = true)
: NameNode(DotToken, Parts, IsValid)
{

    public FullNameNode(NameNode left, NameNode right, OperatorToken dotToken)
        : this(dotToken, left.Parts.Concat(right.Parts).ToImmutableArray(), left.IsValid && right.IsValid) { }
}