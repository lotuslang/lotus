using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public class StringConsumer : IConsumer<char>
{

    public readonly static char EOF = '\u0003';

    public char Default => EOF;

    protected Queue<char> reconsumeQueue;

    protected Stack<char> stack;

    public int Count {
        get => stack.Count + reconsumeQueue.Count;
    }

    private Location lastPosition;

    public char Current { get; protected set; }

    protected Location pos; // we keep it because it's more convenient and makes sense since a character always has an atomic location

    public LocationRange Position {
        get => pos;
    }

    protected StringConsumer() {
        Current = EOF;
        stack = new Stack<char>();
        pos = new Location(1, -1);
        reconsumeQueue = new Queue<char>();
        lastPosition = new Location(0, -1);
    }

    public StringConsumer(IConsumer<char> consumer, string fileName = "<std>") : this() {
        while (consumer.Consume(out char item)) {
            stack.Push(item);
        }

        stack = new Stack<char>(stack);

        pos = new Location(1, -1, filename: fileName);
    }

    public StringConsumer(StringConsumer consumer) : this() {
        stack = new Stack<char>(consumer.stack.Reverse());

        Current = consumer.Current;

        pos = new Location(consumer.pos.line, consumer.pos.column, filename: consumer.pos.filename);

        reconsumeQueue = new Queue<char>(consumer.reconsumeQueue);

        lastPosition = consumer.lastPosition;
    }

    public StringConsumer(IEnumerable<char> collection, string fileName = "<std>") : this() {
        stack = new Stack<char>(collection.Reverse());

        pos = new Location(1, -1, filename: fileName);
    }

    public StringConsumer(FileInfo fileInfo) : this(File.ReadAllLines(fileInfo.FullName), fileInfo.Name)
    { }

    public StringConsumer(IEnumerable<string> lines, string fileName = "<std>") : this() {
        foreach (var line in lines)
        {
            foreach (var ch in line)
            {
                stack.Push(ch);
            }

            stack.Push('\n');
        }

        stack = new Stack<char>(stack);

        pos = new Location(1, -1, filename: fileName);
    }

    public StringConsumer(StreamReader stream, string fileName = "<std>") : this(stream.ReadToEnd().Split('\n'), fileName)
    { }

    public void Reconsume() {

        reconsumeQueue.Enqueue(Current);

        pos = lastPosition;
    }

    public bool Consume([MaybeNullWhen(false)] out char result) {
        result = Consume();

        return result != EOF;
    }

    public char Consume() {

        // If we are instructed to reconsume the last char, then dequeue a char from the reconsumeQueue and return it
        if (reconsumeQueue.Count != 0) {
            if (reconsumeQueue.Peek() == '\n') {
                pos.line++;
                pos.column = -1;
            }

            pos.column++;

            return reconsumeQueue.Dequeue();
        }

        lastPosition = pos;

        if (Count == 0) {
            Current = EOF;

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
            return reconsumeQueue.Peek();
        }

        if (Count == 0) return EOF;

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