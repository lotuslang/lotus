namespace Lotus.Syntax;

public sealed record FullNameNode(OperatorToken DotToken, ImmutableArray<IdentToken> Parts)
: NameNode(DotToken, Parts)
{
    public FullNameNode(NameNode left, NameNode right, OperatorToken dotToken)
        : this(dotToken, left.Parts.AddRange(right.Parts)) {
        IsValid = left.IsValid && right.IsValid;
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IValueVisitor<T> visitor) => visitor.Visit(this);
}