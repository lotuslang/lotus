# Coding Guidelines

If you wish to participate in this project, it is recommended (although not essential) to use a consistent coding style with the rest of the program. This document aims to list and document this style as much as possible. **If it's not written there, it is probably not an explicit style choice. Follow the official C# guidelines if that's the case.** 

## Use spaces with an indentation of 4 thank you.

In most editors that won't make a difference anyway.

## Empty lines

### In method/property bodies

**Empty lines are encouraged to make your code breath.** Empty lines are one of the most important tools available to you to easily separate your code into clear steps.

***Example***

```csharp
int seed = 0;
for (int i = 0; i < random.Next(); i++) {
    if (i % random.Next()) seed += i;
}
Console.WriteLine("Seed for the random person is " + seed);
var person = RandomPerson(seed);
Console.WriteLine(person.LastName);
```

Here, there is no clear difference between the multiple parts of this action :

- Declare a variable to hold a seed

- Choose a seed for the random person generator

- Print the seed

- Generate the random person

- Print its last name

Therefore, it is recommended to break this code as such

```csharp
int seed = 0;

for (int i = 0; i < random.next(); i++) {
    if (i % random.Next()) seed += i;
}

Console.WriteLine("Seed for the random person is : " + seed);

var person = RandomPerson(seed);

Console.WriteLine(person.LastName);
```

**Note :** Yes, variable declaration are *preferred* to be left alone.

As a rule of thumb, if you have a lump of code with more than six consecutive lines of same indentation with breaking, you might want to consider separate it with empty lines. If you're not sure, just ask and I'll do my best to respond =)

**Using double empty lines is not recommended.** If you need to separate two parts of code like that, you might want to extract methods from those parts (when it makes sense, i.e. the method can be reused for other purposes, it doesn't do something *too* specific (naming is hard),  and it doesn't require too many parameters).

### In other situations

**An empty line should be used to separate each method, field, and property in classes.** An empty line should be used to separate the `using`s and the rest of the file.

Other than that, you probably don't need empty lines. 

## Braces

Braces should be put on a new line when used with

- a class declaration

- a namespace statement

- an empty constructor body (in which case they should contain a single space, see [Constructors](#constructors))

- a closure statement (if, switch, for, etc...) whose clause ends at column 120 or more

Otherwise, braces should be on the same line and preceded by a single space.

**Example**

```csharp
using System;
using System.IO;

public class MyClass
{
    public string Name { get; }
    public int Id { get; }

    public MyClass(int id, string name) {
        Id = id;
        Name = name;
    }

    public MyClass(string name) : this(name.GetHasCode(), name)
    { }

    public void PrintCaller(
        [CallerMemberName] string caller = "<unknown caller>",
        [CallerFilePath] string callerPath = ""
    ) {
        if (caller == "") {
            Console.WriteLine("Unknown caller.");
            return;
        }


        Console.WriteLine("Called by " + caller + " @ " + callerPath);
    }



}
```

**Note :** One-liner methods should use the arrow/lambda operator instead of braces, see [Lambdas](#Lambdas). Braces can also be omitted when used in one-liner `if` clauses, see [If clauses  wrapping](#if-clauses-wrapping). 

## If-else statements

### Chaining if-else statements

When being presented with **at least one** of the block that returns or breaks out of an enclosing loop, avoid using the two clauses (`if` and `else`). Instead, use a single clause, as it will avoid using an unnecessary block and indentation.

**Example**

```csharp
if (condition) {
    var dave = new Person(name: "Dave");

    dave.Pseudo = "DaveWave"

    return dave.ToString();
} else {
    var john = new Person(name: "John");

    john.Job = Job.Baker;

    return john.ToString();
}
```

This if-else statement uses two clauses, and branches based on a condition. Both return, therefore it can be abbreviated to

```csharp
if (condition) {
    var dave = new Person(name: "Dave");

    dave.Pseudo = "DaveWave"

    return dave.ToString();
}

var john = new Person(name: "John");

john.Job = Job.Baker;

return john.ToString();
```

**Similarly**, if-else-if clauses can be abbreviated using the same method :

```csharp
if (condition) {
    var dave = new Person(name: "Dave");

    dave.Pseudo = "DaveWave"

    return dave.ToString();
} else if (otherCondition) {
    var john = new Person(name: "John");

    john.Job = Job.Baker;

    // do some other non-returning stuff
} else {
    var bob = new Person(name: "Bob");

    bob.Age = 23;

    return bob.ToString();
}


// continue the non-returning stuff
```

Can become

```csharp
if (condition) {
    var dave = new Person(name: "Dave");

    dave.Pseudo = "DaveWave"

    return dave.ToString();
}

if (!otherCondition) {
    var bob = new Person(name: "Bob");

    bob.Age = 23;

    return bob.ToString();
}

var john = new Person(name: "John");

john.Job = Job.Baker;

// do some other, non returning stuff
```

### Test-and-throw

You will often find yourself testing for a condition, and throwing when it is not fulfilled.

```csharp
if (person.Job != Job.Unemployed) {
   Console.WriteLine(person.Job);
} else {
    throw new Exception();
}
```

Using the previous method as well as some logic, we can transform it to

```csharp
if (person.Job == Job.Unemployed) {
    throw new Exception();
}

Console.WriteLine(person.Job);
```

### If block nesting

It is recommended to avoid nesting too many `if` statements. In that endeavor, you can use the behavior of conditional logical operators like `&&` and `||` :

- `&&` will only evaluate the second operand if the first one is ***true***

- `||` will only evaluate the second operand if the first one is ***false***

See [Boolean logical operators - C# reference | Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/boolean-logical-operators) for more info.

For example, if you need to peek in a potentially empty stack and then use the result if you could peek, instead of using two `if` blocks like that

```csharp
if (personStack.TryPeek(out Person person)) {
    if (person.pseudo.StartsWith("Dave") {
        RemovePerson(person); // fuck dave
        return;
    }
}
```

You can refactor it into this and remove the unnecessary nesting

```csharp
if (personStack.TryPeek(out Person person) && person.StartsWith("Dave")) {
    RemovePerson(person); // fuck dave
    return;
}
```