using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ComplexStringToken : ComplexToken
{
    protected List<Token[]> sections;

    public ReadOnlyCollection<Token[]> CodeSections {
        get => new ReadOnlyCollection<Token[]>(sections);
    }

    public ComplexStringToken(string representation,
                              IList<Token[]> codeSections,
                              Location location,
                              TriviaToken? leading = null,
                              TriviaToken? trailing = null
    )
        : base(representation, TokenKind.complexString, location, leading, trailing)
    {
        sections = new List<Token[]>(codeSections);
    }

    public void AddSection(Token[] section) {
        sections.Add(section);
    }
}