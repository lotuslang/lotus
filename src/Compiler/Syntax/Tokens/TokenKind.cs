namespace Lotus.Syntax;

public enum TokenKind {
    EOF = 0,
    delimiter,
    identifier,
    number,
    @bool,
    @string,
    @char,
    @operator,
    keyword,
    semicolon,
    trivia,
}