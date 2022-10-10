namespace Lotus.Syntax;

#pragma warning disable RCS1154, RCS1157, RCS1191

internal static class ExpressionKindFlags {
    // bit_0 => true: prefix/value
    // bit_1 => true: infix/postfix
    // bit_2 if prefix => true: value, false: prefix op
    // bit_3 if infix  => true: postfix op, false: infix op
    public const int PREFIX  = 0b1000_0000;
    public const int INFIX   = 0b0100_0000;
    public const int VALUE   = 0b1010_0000;
    public const int POSTFIX = 0b0101_0000;
}

[Flags]
public enum ExpressionKind {
    NotAnExpr = 0,

    // values
    Number      = ExpressionKindFlags.VALUE,
    String      = Number + 1,
    Identifier  = String + 1,
    Boolean     = Identifier + 1,

    // value+infix
    LeftParen   = ExpressionKindFlags.VALUE | ExpressionKindFlags.INFIX,

    // prefix only op
    Not         = ExpressionKindFlags.PREFIX,
    New         = Not + 1,

    // prefix+infix ops
    Plus        = ExpressionKindFlags.PREFIX | ExpressionKindFlags.INFIX,
    Minus       = Plus + 1,
    Array       = Minus + 1,

    // prefix+postfix ops
    Increment   = ExpressionKindFlags.PREFIX | ExpressionKindFlags.POSTFIX,
    Decrement   = Increment + 1,

    // infix
    Multiply    = ExpressionKindFlags.INFIX,
    Divide      = Multiply + 1,
    Modulo      = Divide + 1,
    Power       = Modulo + 1,
    Access      = Power + 1,
    Eq          = Access + 1,
    NotEq       = Eq + 1,
    Or          = NotEq + 1,
    And         = Or + 1,
    Xor         = And + 1,
    Greater     = Xor + 1,
    Less        = Greater + 1,
    GreaterOrEq = Less + 1,
    LessOrEq    = GreaterOrEq + 1,
    Assignment  = LessOrEq + 1,
    Ternary     = Assignment + 1,
}