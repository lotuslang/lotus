using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

[System.Diagnostics.DebuggerDisplay("{loc} {kind} : {rep.ToString()}")]
public class Token
{
    protected TokenKind kind;

    public TokenKind Kind {
        get => kind;
    }

    protected StringBuilder rep;

    public string Representation {
        get => rep.ToString();
    }

    Location? loc;

    public Location? Location {
        get => loc;
    }

    public Token(char representation, TokenKind kind, Location? location) : this(representation.ToString(), kind, location) { }

    public Token(string representation, TokenKind kind, Location? location) {
        rep = new StringBuilder();
        rep.Append(representation);
        this.kind = kind;
        loc = location;
    }

    public override string ToString() {
        return rep.ToString();
    }

    public static implicit operator TokenKind(Token token) {
        return token.kind;
    }

    public static implicit operator string(Token token) {
        return token.rep.ToString();
    }
}

public class ComplexToken : Token
{
    public ComplexToken(string representation, TokenKind kind, Location? location) : base(representation, kind, location) { }

    public void Add(char ch) => rep.Append(ch);

    public void Add(string str) => rep.Append(str);
}

[System.Diagnostics.DebuggerDisplay("{loc} {kind} : {val}")]
public class NumberToken : ComplexToken
{
    protected double val;

    public double Value {
        get => val;
    }

    public NumberToken(string representation, Location? location) : base(representation, TokenKind.number, location) {
        rep = new StringBuilder(representation);
        Double.TryParse(representation, out val);
    }

    public new void Add(char ch) {
        rep.Append(ch);
        Double.TryParse(rep.ToString(), out val);
    }

    public new void Add(string str) {
        rep.Append(str);
        Double.TryParse(rep.ToString(), out val);
    }
}

[System.Diagnostics.DebuggerDisplay("{loc} {kind} : {val}")]
public class BoolToken : ComplexToken
{
    protected bool val;

    public bool Value {
        get => val;
    }

    public BoolToken(string representation, Location? location) : base(representation, TokenKind.number, location) {
        if (representation != "true" || representation != "false") {
            throw new Exception($"Unexpected representation of a bool token : {representation}. Expected `true` of `false`");
        }

        val = representation == "true";
    }

    public BoolToken(bool value, Location? location) : base(value.ToString().ToLower(), TokenKind.@bool, location) {

    }
}

[System.Diagnostics.DebuggerDisplay("{loc} {kind} : {val}")]
public class ComplexStringToken : ComplexToken
{
    protected string val;

    public string Value {
        get => val;
    }

    protected List<Token[]> sections;

    public List<Token[]> CodeSections;

    public ComplexStringToken(string representation, List<Token[]> codeSections, Location? location) : base(representation, TokenKind.complexString, location) {
        sections = codeSections;
    }
}

[System.Diagnostics.DebuggerDisplay("{loc} {kind} : {rep.ToString()} with precedence {precedence}")]
public class OperatorToken : Token
{
    protected Precedence precedence;

    public Precedence Precedence {
        get => precedence;
    }

    protected bool isLeft;

    public bool IsLeftAssociative {
        get => isLeft;
    }


    public OperatorToken(string representation, Precedence precedence, bool isLeftAssociative, Location? location)
        : base(representation, TokenKind.@operator, location)
    {
        this.precedence = precedence;
        isLeft = isLeftAssociative;
    }

    public OperatorToken(string representation, Precedence precedence, string associativity, Location? location)
        : this(representation, precedence, associativity == "left" ? true : false, location)
    { }

    public OperatorToken(char representation, Precedence precedence, string associativity, Location? location)
        : this(representation.ToString(), precedence, associativity == "left" ? true : false, location)
    { }

    public OperatorToken(char representation, Precedence precedence, bool isLeftAssociative, Location? location)
        : this(representation.ToString(), precedence, isLeftAssociative, location)
    { }
}

public enum TokenKind {
    delim, ident, number, function, @bool, @string, complexString, @operator, keyword, EOF
}