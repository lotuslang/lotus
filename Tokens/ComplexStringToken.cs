using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public record ComplexStringToken(string Representation, List<Token[]> CodeSections, LocationRange Location, bool IsValid = true)
: Token(Representation, TokenKind.@string, Location, IsValid)
{
    public new static readonly ComplexStringToken NULL = new("", new List<Token[]>(), LocationRange.NULL, false);

    public void AddSection(Token[] section)
        => CodeSections.Add(section);

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}