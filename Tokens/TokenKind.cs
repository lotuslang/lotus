public enum TokenKind {
    EOF = 0,
    delimiter,
    identifier,
    number,
    @bool,
    @string,
    complexString,
    @operator,
    keyword,
    trivia,
}