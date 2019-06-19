using System;
using System.IO;

class Program
{
    static void Main(string[] args) {

        var lines = File.ReadAllLines(Directory.GetCurrentDirectory() + "/sample.txt");

        // Initializes the tokenizer with the content of the "sample.txt" file
        var tokenizer = new Tokenizer(lines);

        // Temp variable to store the result of tokenizer.Consume
        var success = true;

        // Repeatedly consumes a token until we can't anymore
        while (success) {
            var token = tokenizer.Consume(out success);
            Console.WriteLine($"{token.Location} {token.Kind} : {token.Representation}");
        }

        // Resets the tokenizer
        tokenizer = new Tokenizer(lines);

        Console.WriteLine();

        // Initializes the parser with the (reseted) tokenizer
        //var parser = new Parser(tokenizer);
    }
}