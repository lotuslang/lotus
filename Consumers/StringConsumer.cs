using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public class StringConsumer : IConsumer<char>
{

    public readonly static char EOF = '\u0003';

    public char Default => EOF;

    protected Queue<char> reconsumeQueue;

    protected Stack<char> stack;

    public int Count => stack.Count + reconsumeQueue.Count;

    private Location lastPosition;

    public char Current { get; protected set; }

    protected Location pos; // we keep it because it's more convenient and makes sense since a character always has an atomic location

	public LocationRange Position => pos;

    protected StringConsumer() {
        Current = EOF;
        stack = new Stack<char>();
        pos = new Location(1, -1);
        reconsumeQueue = new Queue<char>();
        lastPosition = new Location(0, -1);
    }

    public StringConsumer(IConsumer<char> consumer, string fileName = "<std>") : this() {
        while (consumer.Consume(out var item)) {
            stack.Push(item);
        }

        stack = new Stack<char>(stack);

        pos = new Location(1, -1, fileName);
    }

    public StringConsumer(StringConsumer consumer) : this() {
		stack = consumer.stack.Clone();

        Current = consumer.Current;

        pos = new Location(consumer.pos.line, consumer.pos.column, consumer.pos.filename);

        reconsumeQueue = new Queue<char>(consumer.reconsumeQueue);

        lastPosition = consumer.lastPosition;
    }

    public StringConsumer(IEnumerable<char> collection, string fileName = "<std>") : this() {
        stack = new Stack<char>(collection.Reverse());

        pos = new Location(1, -1, fileName);
    }

    public StringConsumer(Uri fileInfo) : this(File.ReadAllLines(fileInfo.AbsolutePath), fileInfo.AbsolutePath)
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

        pos = new Location(1, -1, fileName);
    }

    public StringConsumer(StreamReader stream, string fileName = "<std>") : this(stream.ReadToEnd(), fileName)
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
                UpdatePosForNewline();
            }

             pos = pos with { column = pos.column + 1 };

            return reconsumeQueue.Dequeue();
        }

        lastPosition = pos;

        if (Count == 0) {
            Current = EOF;

            return Current;
        }

        Current = stack.Pop();

        if (Current == '\n') {
            UpdatePosForNewline();
        }

        pos = pos with { column = pos.column + 1 };

        return Current;
    }

    public char Peek() {

        if (reconsumeQueue.Count != 0) {
            return reconsumeQueue.Peek();
        }

        return Count == 0 ? EOF : stack.Peek();
    }

    public char[] Peek(int n) {
        var output = new char[n];

        var consumer = new Consumer<char>(this);

        for (int i = 0; i < n; i++) {
            output[i] = consumer.Consume();
        }

        return output;
    }

    private void UpdatePosForNewline() => pos = pos with { line = pos.line + 1, column = -1 };
}