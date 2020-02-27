using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

public class StringConsumer : IConsumer<char>
{
    protected Queue<char> reconsumeQueue;

    protected Stack<char> stack;

    public int Count {
        get => stack.Count;
    }

    public char Current { get; protected set; }

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

        pos = new Location(1, 0, fileName);

        reconsumeQueue = new Queue<char>();
    }

    public StringConsumer(StringConsumer consumer) {
        stack = new Stack<char>(consumer.stack.Reverse());

        Current = consumer.Current;

        pos = new Location(consumer.pos.line, consumer.pos.column, consumer.pos.filename);

        reconsumeQueue = new Queue<char>(consumer.reconsumeQueue);
    }

    public StringConsumer(IEnumerable<char> collection, string fileName = "<std>") {
        stack = new Stack<char>(collection.Reverse());

        pos = new Location(1, 0, fileName);

        reconsumeQueue = new Queue<char>();
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

        pos = new Location(1, 0, fileName);

        reconsumeQueue = new Queue<char>();
    }

    public StringConsumer(StreamReader stream, string fileName = "<std>") : this(stream.ReadToEnd().Split('\n'), fileName)
    { }

    public void Reconsume() {

        reconsumeQueue.Enqueue(Current);

        if (Current == '\n') {
            pos.line--;
            pos.column = -1;
        }

        pos.column++;
    }

    public bool Consume(out char result) {
        result = Consume();

        return result != '\u0003';
    }

    public char Consume() {

        // If we are instructed to reconsume the last char, then dequeue a char from the reconsumeQueue and return it
        if (reconsumeQueue.Count != 0) {
            return reconsumeQueue.Dequeue();
        }

        if (Count <= 0) {
            Current = '\u0003';

            return Current;
        }

        Current = stack.Pop();

        if (Current == '\n') {
            pos.line++;
            pos.column = -1;
        }

        pos.column++;

        return Current;
    }

    public char Peek() {

        if (reconsumeQueue.Count != 0) {
            reconsumeQueue.Peek();
        }

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