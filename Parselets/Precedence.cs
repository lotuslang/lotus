/*

Here's a beautiful ASCII table for operator precedence

+------------+--------------+-------------------------------------------+
| Precedence | Operator(s)  | Description                               |
+------------+--------------+-------------------------------------------+
|            | a()          | Function call                             |
| 11         | a[]          | Array access                              |
|            | .            | Member access                             |
+------------+--------------+-------------------------------------------+
|            | ++a --a      | Prefix increment and decrement            |
| 10         | a++ a--      | Postfix increment and decrement           |
|            | (type)object | Type casting                              |
+------------+--------------+-------------------------------------------+
| 9          | ^            | Exponentiation                            |
+------------+--------------+-------------------------------------------+
| 8          | a*b a/b a%b  | Multiplication, division and modulo       |
+------------+--------------+-------------------------------------------+
| 7          | a+b a-b      | Addition and Substraction                 |
+------------+--------------+-------------------------------------------+
| 6          | < <=         | Greater-than and greater-than-or-equal-to |
|            | > >=         | Lesser-than and lesser-than-or-equal-to   |
+------------+--------------+-------------------------------------------+
| 5          | == !=        | equal-to and not-equal-to                 |
+------------+--------------+-------------------------------------------+
| 4          | ^^           | Logical XOR                               |
+------------+--------------+-------------------------------------------+
| 3          | &&           | Logical AND                               |
+------------+--------------+-------------------------------------------+
| 2          | ||           | Logical OR                                |
+------------+--------------+-------------------------------------------+
| 1          | var a = b    | Declaration                               |
|            | a = c        | Assignment                                |
+------------+--------------+-------------------------------------------+
| 0          | ,            | Comma                                     |
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
    Assignment = 1,
    Declaration = Assignment,
    Or = 2,
    And = 3,
    Xor = 4,
    Equal = 5,
    NotEqual = Equal,
    LessThan = 6,
    GreaterThan = LessThan,
    LessThanOrEqual = LessThan,
    GreaterThanOrEqual = LessThan,
    Addition = 7,
    Substraction = Addition,
    Multiplication = 8,
    Division = Multiplication,
    Modulo = Multiplication,
    Power = 9,
    Unary = 10,
    TypeCast = Unary,
    Access = 11,
    FuncCall = Access,
    ArrayAccess = Access,
}