using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

public class StringConsumer : IConsumer<char>
{
    private bool reconsumeFlag;

    protected Stack<char> stack;

    public int Count {
        get => stack.Count;
    }

    protected char current;

    public char Current {
        get => current;
    }

    protected Location pos;

    public Location Position {
        get => pos;
    }

    public StringConsumer(IConsumer<char> consumer, string fileName = "<std>") {
        stack = new Stack<char>();

        while (consumer.Consume(out char item)) {
            stack.Push(item);
        }

        stack = new Stack<char>(stack);

        pos.line = 1;
        pos.column = 1;
        pos.filename = fileName;
    }

    public StringConsumer(StringConsumer consumer) {
        stack = new Stack<char>(consumer.stack.Reverse());

        current = consumer.current;

        reconsumeFlag = consumer.reconsumeFlag;

        pos.line = consumer.pos.line;
        pos.column = consumer.pos.column;
        pos.filename = consumer.pos.filename;
    }

    public StringConsumer(IEnumerable<char> collection, string fileName = "<std>") {
        stack = new Stack<char>(collection.Reverse());

        pos.line = 1;
        pos.column = 1;
        pos.filename = fileName;
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

        pos.line = 1;
        pos.column = 1;
        pos.filename = fileName;
    }

    public StringConsumer(StreamReader stream, string fileName = "<std>") : this(stream.ReadToEnd().Split('\n'), fileName)
    { }

    public void Reconsume() {

        if (reconsumeFlag) return;

        reconsumeFlag = true;

        if (current == '\n') {
            pos.line--;
        }

        pos.column--;
    }

    public bool Consume(out char result) {
        result = Consume();

        return result != '\u0003';
    }

    public char Consume() {
        if (reconsumeFlag) {
            reconsumeFlag = false;
            return current;
        }

        if (Count <= 0) {
            current = '\u0003';

            return current;
        }

        current = stack.Pop();

        if (current == '\n') {
            pos.line++;
            pos.column = 0;
        }

        pos.column++;

        return current;
    }

    public char Peek() {
        if (reconsumeFlag) return current;

        if (Count == 0) return '\u0003';

        return stack.Peek();
    }

    public char[] Peek(int n) {
        var output = new char[n];

        var consumer = new Consumer<char>(this);

        for (int i = 0; i < n; i++) {
            output[i] = consumer.Consume();
        }

        return output;
    }
}