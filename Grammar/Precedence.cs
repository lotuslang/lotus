/*

Here's a beautiful ASCII table for operator precedence

+------------+--------------+-------------------------------------------+
| Precedence | Operator(s)  | Description                               |
+------------+--------------+-------------------------------------------+
| 13         | a[]          | Array access                              |
|            | a.b          | Member access                             |
+------------+--------------+-------------------------------------------+
| 12         | a()          | Function call                             |
+------------+--------------+-------------------------------------------+
|            | ++a --a      | Prefix increment and decrement            |
| 11         | a++ a--      | Postfix increment and decrement           |
|            | (type)object | Type casting                              |
+------------+--------------+-------------------------------------------+
| 10         | ^            | Exponentiation                            |
+------------+--------------+-------------------------------------------+
| 9          | a*b a/b a%b  | Multiplication, division and modulo       |
+------------+--------------+-------------------------------------------+
| 8          | a+b a-b      | Addition and Substraction                 |
+------------+--------------+-------------------------------------------+
| 7          | < <=         | Greater-than and greater-than-or-equal-to |
|            | > >=         | Lesser-than and lesser-than-or-equal-to   |
+------------+--------------+-------------------------------------------+
| 6          | == !=        | equal-to and not-equal-to                 |
+------------+--------------+-------------------------------------------+
| 5          | ^^           | Logical XOR                               |
+------------+--------------+-------------------------------------------+
| 4          | &&           | Logical AND                               |
+------------+--------------+-------------------------------------------+
| 3          | ||           | Logical OR                                |
+------------+--------------+-------------------------------------------+
| 2          | a ? b : c    | Ternary (=conditional) operator           |
+------------+--------------+-------------------------------------------+
| 1          | var a = b    | Declaration                               |
|            | a = c        | Assignment                                |
+------------+--------------+-------------------------------------------+
| 0          | ,            | Comma                                     |
|            | a::b         | Inheritance                               |
+------------+--------------+-------------------------------------------+

Made with tablesgenerator.com

Note : Highest precedence means that they are parenthesized before the lower ones.
For example :

6 / 9 + 3 * 8 > Random.getNext() * 100

becomes this :

((6 / 9) + (3 * 8)) > (((Random.getNext)()) * 100)

Or, using a tree :

'>'
 |--'+'
 |   |--'/'
 |   |   |--6
 |   |   |
 |   |   |--9
 |   |
 |   |--'*'
 |   |   |--3
 |   |   |
 |   |   |--8
 |
 |--'*'
 |   |--function-call
 |   |  |--'.'
 |   |  |   |--Random
 |   |  |   |
 |   |  |   |--getNext
 |   |
 |   |--100

Made by hand ;)

*/

// PLEASE UPDATE TABLE IF YOU CHANGE THIS CLASS
public enum Precedence {
    Comma = 0,
    Parenthesis = Comma,
    Curly = Comma,
    DoubleColon = Comma,

    Assignment = Comma + 1,
    Declaration = Assignment,

    Ternary = Assignment + 1,

    Or = Ternary + 1,

    And = Or + 1,

    Xor = And + 1,

    Equal = Xor + 1,
    NotEqual = Equal,

    LessThan = Equal + 1,
    GreaterThan = LessThan,
    LessThanOrEqual = LessThan,
    GreaterThanOrEqual = LessThan,

    Addition = LessThan + 1,
    Substraction = Addition,

    Multiplication = Addition + 1,
    Division = Multiplication,
    Modulo = Multiplication,

    Power = Multiplication + 1,

    Unary = Power + 1,
    TypeCast = Unary,

    FuncCall = Unary + 1,

    Access = FuncCall + 1,
    ArrayAccess = Access,
}