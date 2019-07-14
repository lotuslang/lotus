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

    protected string file;

    public Location Position {
        get => new Location(line, column, file);
    }

    public StringConsumer(IConsumer<char> consumer, string fileName = "<std>") {
        stack = new Stack<char>();

        var success = true;

        while (success) {
            stack.Push(consumer.Consume(out success));
        }

        stack = new Stack<char>(stack);

        line = 1;
        column = 1;
        file = fileName;
    }

    public StringConsumer(StringConsumer consumer) {
        stack = new Stack<char>(consumer.stack.Reverse());

        line = consumer.line;
        column = consumer.column;
        file = consumer.file;
    }

    public StringConsumer(IEnumerable<char> collection, string fileName = "<std>") {
        stack = new Stack<char>(collection.Reverse());

        line = 1;
        column = 1;
        file = fileName;
    }

    public StringConsumer(FileInfo fileInfo) : this(File.ReadAllLines(fileInfo.FullName), fileInfo.Name)
    { }

    public StringConsumer(IEnumerable<string> lines, string fileName = "<std>") {
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
        file = fileName;
    }

    public StringConsumer(StreamReader stream, string fileName = "<std>") : this(stream.ReadToEnd().Split('\n'), fileName)
    { }

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