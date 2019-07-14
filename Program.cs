using System;
using System.IO;

class Program
{
    static void Main(string[] args) {

        // Initializes the tokenizer with the content of the "sample.txt" file
        var tokenizer = new Tokenizer(new FileInfo(Directory.GetCurrentDirectory() + "/sample.txt"));

        // Temp variable to store the result of tokenizer.Consume
        var success = true;

        // Repeatedly consumes a token until we can't anymore
        while (success) {
            var token = tokenizer.Consume(out success);
            //Console.WriteLine($"{token.Location} {token.Kind} : {token.Representation}");
        }

        // Resets the tokenizer
        tokenizer = new Tokenizer(new FileInfo(Directory.GetCurrentDirectory() + "/sample.txt"));

        var parser = new Parser(tokenizer);

        var expr = "sin (-58 * -(69 * MINVAL), rnd.next()) * math.const.pi + - (max('hello'.length, indexer.val++)).toStr()";
        Console.WriteLine(expr);
        Console.WriteLine(String.Join(" ", Parser.ToPostfixNotation(new Tokenizer(expr)).ConvertAll(t => {
            if (t == "++" || t == "--") {
                return ((t as OperatorToken).IsLeftAssociative ? "(postfix)" : "(prefix)") + (t.Representation);
            }

            return (t.Representation);
        })));

        var val = new Parser(new Tokenizer(expr)).ConsumeValue();

        var g = new Graph("G");

        g.AddNode(val.ToGraphNode());

        Console.WriteLine(g.ToText());


        // DEBUG
        //tokens.ForEach(t => Console.WriteLine(t));

        // Initializes the parser with the (reseted) tokenizer
        //var parser = new Parser(tokenizer);
    }
}