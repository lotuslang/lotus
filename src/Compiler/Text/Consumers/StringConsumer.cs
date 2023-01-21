using System.IO;

namespace Lotus.Text;

public sealed class StringConsumer : Consumer<char>, ISourceCodeProvider
{
    public readonly static char EOF = '\u0003';

    public string Filename => pos.filename;

    private readonly Lazy<SourceCode> _src;
    public SourceCode Source => _src.Value;

    private Location lastPos;

    private Location pos; // we keep it because it's more convenient and makes sense since a character always has an atomic location

	public override LocationRange Position => pos;

    private StringConsumer() : base() {
        pos = new Location(1, 0);
        lastPos = new Location(1, 0);

        // _data will be fetched after init, so it's fine
        _src = new Lazy<SourceCode>(() => new SourceCode(new string(_data.AsSpan())), isThreadSafe: false);
    }

    public StringConsumer(StringConsumer consumer) : this() {
		_data = consumer._data;
        _currIdx = consumer._currIdx;

        // Since Location is a record, the underlying value never mutates
        pos = consumer.pos;

        lastPos = consumer.lastPos;
    }

    public StringConsumer(string str, string fileName = "<std>") : this() {
        _data = str.ToImmutableArray();

        pos = pos with { filename = fileName };
    }

    public StringConsumer(IConsumer<char> consumer, string fileName = "<std>") : this() {
        _data = consumer.ToImmutableArray();

        pos = pos with { filename = fileName };
    }

    public StringConsumer(IEnumerable<char> collection, string fileName = "<std>") : this() {
        _data = collection.ToImmutableArray();

        pos = pos with { filename = fileName };
    }

    public StringConsumer(Uri fileInfo) : this(File.ReadAllText(fileInfo.AbsolutePath), fileInfo.AbsolutePath)
    { }

    public StringConsumer(StreamReader stream, string fileName = "<std>") : this(stream.ReadToEnd(), fileName)
    { }

    public override void Reconsume() {
        base.Reconsume();
        pos = lastPos;
    }

    public override ref readonly char Consume() {
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