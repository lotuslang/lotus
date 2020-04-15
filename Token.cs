using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[System.Diagnostics.DebuggerDisplay("{Location} {Kind} : {rep.ToString()}")]
public class Token
{
    public static readonly Token NULL = new Token('\0', TokenKind.EOF, default(Location), null, null);

    public TriviaToken LeadingTrivia { get; protected set; }

    public TriviaToken TrailingTrivia { get; protected set; }

    public TokenKind Kind { get; protected set; }

    protected StringBuilder rep;

    public string Representation {
        get => rep.ToString();
    }

    public Location? Location { get; protected set; }

    public Token(char representation, TokenKind kind, Location? location, TriviaToken leading = null, TriviaToken trailing = null)
        : this(representation.ToString(), kind, location, leading, trailing) { }

    public Token(string representation, TokenKind kind, Location? location, TriviaToken leading = null, TriviaToken trailing = null) {
        rep = new StringBuilder();
        rep.Append(representation);
        Kind = kind;
        Location = location;
        LeadingTrivia = leading;
        TrailingTrivia = trailing;
    }

    public void AddLeadingTrivia(TriviaToken trivia) {
        if (trivia is null) {
            throw new ArgumentNullException(nameof(trivia));
        }

        if (LeadingTrivia is null)
            LeadingTrivia = trivia;
        else
            LeadingTrivia.AddLeadingTrivia(trivia);
    }

    public void AddTrailingTrivia(TriviaToken trivia) {
        if (trivia is null) {
            throw new ArgumentNullException(nameof(trivia));
        }

        if (TrailingTrivia is null)
            TrailingTrivia = trivia;
        else
            TrailingTrivia.AddTrailingTrivia(trivia);
    }

    public override string ToString() {
        return rep.ToString();
    }

    public static implicit operator TokenKind(Token token) {
        return token.Kind;
    }

    public static implicit operator string(Token token) {
        return token.rep.ToString();
    }
}

public class ComplexToken : Token
{
    public ComplexToken(string representation, TokenKind kind, Location? location, TriviaToken leading = null, TriviaToken trailing = null)
        : base(representation, kind, location, leading, trailing) { }

    public void Add(char ch)
        => rep.Append(ch);

    public void Add(string str)
        => rep.Append(str);
}

[System.Diagnostics.DebuggerDisplay("{Location} {Kind} : {val}")]
public class NumberToken : ComplexToken
{
    protected double val;

    public double Value { get => val; }

    public NumberToken(string representation, Location? location, TriviaToken leading = null, TriviaToken trailing = null)
        : base(representation, TokenKind.number, location, leading, trailing)
    {
        if (representation.Length != 0 && !Double.TryParse(representation, out val)) throw new Exception();
    }

    // we should keep them because it's possible for someone to call ComplexToken.Add() on this instance
    // so that it wouldn't be a valid number.
    // However, in the state they are right now, they cannot really be used to create a number, so meh.
    public new void Add(char ch) {
        rep.Append(ch);
        if (!Double.TryParse(Representation, out val)) throw new Exception();
    }

    public new void Add(string str) {
        rep.Append(str);
        if (!Double.TryParse(Representation, out val)) throw new Exception();
    }
}

[System.Diagnostics.DebuggerDisplay("{Location} {Kind} : {val}")]
public class BoolToken : ComplexToken
{
    protected bool val;

    public bool Value {
        get => val;
    }

    public BoolToken(string representation, Location? location, TriviaToken leading = null, TriviaToken trailing = null)
        : base(representation, TokenKind.number, location, leading, trailing)
    {
        if (representation.Length != 0 && !Boolean.TryParse(representation, out val)) throw new Exception();
    }

    public BoolToken(bool value, Location? location) : base(value.ToString().ToLower(), TokenKind.@bool, location) {

    }
}

public class ComplexStringToken : ComplexToken
{
    protected List<Token[]> sections;

    public ReadOnlyCollection<Token[]> CodeSections {
        get => new ReadOnlyCollection<Token[]>(sections);
    }

    public ComplexStringToken(string representation, List<Token[]> codeSections, Location? location, TriviaToken leading = null, TriviaToken trailing = null)
        : base(representation, TokenKind.complexString, location, leading, trailing)
    {
        sections = codeSections;
    }

    public void AddSection(Token[] section) {
        sections.Add(section);
    }
}

[System.Diagnostics.DebuggerDisplay("{Location} {Precedence}({(int)Precedence}) : {Representation}")]
public class OperatorToken : Token
{
    public Precedence Precedence { get; protected set; }

    protected bool isLeft;

    public bool IsLeftAssociative {
        get => isLeft;
    }


    public OperatorToken(string representation, Precedence precedence, bool isLeftAssociative, Location? location)
        : base(representation, TokenKind.@operator, location)
    {
        Precedence = precedence;
        isLeft = isLeftAssociative;
    }

    public OperatorToken(string representation, Precedence precedence, string associativity, Location? location)
        : this(representation, precedence, associativity == "left", location)
    { }

    public OperatorToken(char representation, Precedence precedence, string associativity, Location? location)
        : this(representation.ToString(), precedence, associativity == "left", location)
    { }

    public OperatorToken(char representation, Precedence precedence, bool isLeftAssociative, Location? location)
        : this(representation.ToString(), precedence, isLeftAssociative, location)
    { }

    public OperatorToken(Token token, Precedence precedence, string associativity, Location? location)
        : this(token, precedence, associativity == "left", location)
    {
        Kind = token.Kind == TokenKind.function ? TokenKind.function : TokenKind.@operator;
    }
}

public enum TokenKind {
    delim, ident, number, function, @bool, @string, complexString, @operator, keyword, EOF, trivia
}