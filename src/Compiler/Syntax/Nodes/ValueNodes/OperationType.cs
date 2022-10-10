namespace Lotus.Syntax;

public enum OperationType {
    Unknown = 0,
    Positive, // +a
    Negative, // -a
    Not, // !a
    PrefixIncrement, // ++a
    PostfixIncrement, // a++
    PrefixDecrement, // --a
    PostfixDecrement, // a--
    Addition, // a + b
    Subtraction, // a - b
    Multiplication, // a * b
    Division, // a / b
    Power, // a ^ b
    Modulo, // a % b
    Or, // a || b
    And, // a && b
    Xor, // a ^^ b
    Equal, // a == b
    NotEqual, // a != b
    Less, // a < b
    LessOrEqual, // a <= b
    Greater, // a > b
    GreaterOrEqual, // a >= b
    Access, // a.b
    Assign, // a = b
    ArrayAccess, // a[b]
    Conditional, // a ? b : c
}