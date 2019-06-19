using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

public class StringConsumer : IConsumer<char>
{
    protected Stack<char> stack;

    public int Count {
        get => stack.Count;
    }

    protected char current;

    public char Current {
        get => current;
    }

    protected int line;
    protected int column;

    public Location Position {
        get => new Location(line, column);
    }

    public StringConsumer(IConsumer<char> consumer) {
        stack = new Stack<char>();

        var success = true;

        while (success) {
            stack.Push(consumer.Consume(out success));
        }

        stack = new Stack<char>(stack);

        line = 1;
        column = 1;
    }

    public StringConsumer(IEnumerable<char> collection) {
        stack = new Stack<char>(collection.Reverse());

        line = 1;
        column = 1;
    }

    public StringConsumer(string path) : this(File.ReadAllLines(path)) { }

    public StringConsumer(IEnumerable<string> lines) {
        stack = new Stack<char>();

        foreach (var line in lines)
        {
            foreach (var ch in line)
            {
                stack.Push(ch);
            }

            stack.Push('\n');
        }

        stack = new Stack<char>(stack);

        line = 1;
        column = 1;
    }

    public StringConsumer(StreamReader stream) : this(stream.ReadToEnd().Split('\n')) { }

    public void Reconsume() {
        stack.Push(current);

        if (current == '\n') {
            line--;
        }

        column--;
    }

    public char Consume(out bool success) {
        var temp = Consume();

        success = temp != '\u0003';

        return temp;
    }

    public char Consume() {
        if (Count <= 0) {
            current = '\u0003';

            return current;
        }

        current = stack.Pop();

        if (current == '\n') {
            line++;
            column = 0;
        }

        column++;

        return current;
    }

    public char Peek() {
        if (Count <= 0) return '\u0003';

        return stack.Peek();
    }
}