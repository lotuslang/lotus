/*

Here's a beautiful ASCII table for operator precedence

+------------+--------------+-------------------------------------------+
| Precedence | Operator(s)  | Description                               |
+------------+--------------+-------------------------------------------+
|            | a()          | Function call                             |
| 10         | a[]          | Array access                              |
|            | .            | Member access                             |
+------------+--------------+-------------------------------------------+
|            | ++a --a      | Prefix increment and decrement            |
| 9          | a++ a--      | Postfix increment and decrement           |
|            | (type)object | Type casting                              |
+------------+--------------+-------------------------------------------+
| 8          | ^            | Exponentiation                            |
+------------+--------------+-------------------------------------------+
| 7          | a*b a/b a%b  | Multiplication, division and modulo       |
+------------+--------------+-------------------------------------------+
| 6          | a+b a-b      | Addition and Substraction                 |
+------------+--------------+-------------------------------------------+
| 5          | < <=         | Greater-than and greater-than-or-equal-to |
|            | > >=         | Lesser-than and lesser-than-or-equal-to   |
+------------+--------------+-------------------------------------------+
| 4          | == !=        | equal-to and not-equal-to                 |
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

public enum Precedence {
    Comma = 0,
    Parenthesis = Comma,
    Curly = Comma,
    Assignment = 1,
    Declaration = Assignment,
    Or = 2,
    And = 3,
    Equal = 4,
    NotEqual = Equal,
    LessThan = 5,
    GreaterThan = LessThan,
    LessThanOrEqual = LessThan,
    GreaterThanOrEqual = LessThan,
    Addition = 6,
    Substraction = Addition,
    Multiplication = 7,
    Division = Multiplication,
    Modulo = Multiplication,
    Power = 8,
    Unary = 9,
    TypeCast = Unary,
    Access = 10,
    FuncCall = Access,
    ArrayAccess = Access,
}