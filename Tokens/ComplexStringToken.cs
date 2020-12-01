using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ComplexStringToken : ComplexToken
{
    public new static readonly ComplexStringToken NULL = new ComplexStringToken("", new Token[0][], LocationRange.NULL, false);

    protected List<Token[]> sections;

    public ReadOnlyCollection<Token[]> CodeSections {
        get => new ReadOnlyCollection<Token[]>(sections);
    }

    public ComplexStringToken(string representation,
                              IList<Token[]> codeSections,
                              LocationRange location,
                              bool isValid = true,
                              TriviaToken? leading = null,
                              TriviaToken? trailing = null
    )
        : base(representation, TokenKind.complexString, location, isValid, leading, trailing)
    {
        sections = new List<Token[]>(codeSections);
    }

    public void AddSection(Token[] section) {
        sections.Add(section);
    }

    public override T Accept<T>(TokenVisitor<T> visitor) => visitor.Visit(this);
}