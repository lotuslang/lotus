public enum TokenKind {
    EOF = 0,
    delim,
    ident,
    number,
    function,
    @bool,
    @string,
    complexString,
    @operator,
    keyword,
    trivia,
}