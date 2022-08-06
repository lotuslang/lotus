using System.IO;

public sealed class StringConsumer : Consumer<char>
{
    public readonly static char EOF = '\u0003';

    private new Location lastPos;

    private Location pos; // we keep it because it's more convenient and makes sense since a character always has an atomic location

	public override LocationRange Position => pos;

    private new void Init() {
        _data = Array.Empty<char>();
        _atStart = true;
        pos = new Location(1, 0);
        lastPos = new Location(1, 0);
    }

#nullable disable
    private StringConsumer() : base() {
        Init();
    }

    public StringConsumer(StringConsumer consumer) : this() {
		_data = consumer._data;
        _currIdx = consumer._currIdx;

        _atStart = consumer._atStart;

        // Since Location is a record, the underlying value never mutates
        pos = consumer.pos;

        lastPos = consumer.lastPos;
    }

    public StringConsumer(IEnumerable<char> collection, string fileName = "<std>") : this() {
        _data = collection.ToArray();

        pos = new Location(1, 0, fileName);
    }

    public StringConsumer(Uri fileInfo) : this(File.ReadAllText(fileInfo.AbsolutePath), fileInfo.AbsolutePath)
    { }

    public StringConsumer(IEnumerable<string> lines, string fileName = "<std>") : this() {
        _data = String.Join('\n', lines).ToCharArray();

        pos = new Location(1, 0, fileName);
    }

    public StringConsumer(StreamReader stream, string fileName = "<std>") : this(stream.ReadToEnd(), fileName)
    { }
#nullable restore

    public override void Reconsume() {
        base.Reconsume();
        pos = lastPos;
    }

    public override ref readonly char Consume() {

        // If we are instructed to reconsume the last char, then dequeue a char from the reconsumeQueue and return it
        lastPos = pos;

        ref readonly var output = ref base.Consume();

        if (output == '\n')
            UpdatePosForNewline();

        pos = pos with { column = pos.column + 1 };

        return ref output;
    }

    public override StringConsumer Clone() => new(this);

    private void UpdatePosForNewline() => pos = pos with { line = pos.line + 1, column = -1 };
}