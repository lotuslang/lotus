public record FullNameNode(OperatorToken DotToken, IList<IdentToken> Parts, bool IsValid = true)
: NameNode(DotToken, Parts, IsValid)
{

    public FullNameNode(NameNode left, NameNode right, OperatorToken dotToken)
        : this(dotToken, left.Parts.Concat(right.Parts).ToArray(), left.IsValid && right.IsValid) { }
}