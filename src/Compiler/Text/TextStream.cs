using System.Collections;

namespace Lotus.Text;

public sealed class TextStream : ISourceCodeProvider, IEnumerable<char>, IEnumerator<char>
{
    [DebuggerDisplay("[l: {Length,nq}] {_str}")]
    private readonly struct Line {
        private readonly string _str;

        public readonly int Length => _str.Length + 1;
        public readonly char this[int idx]
            => idx < _str.Length ? _str[idx] : '\n';

        public Line(string s) => _str = s;
    }

    private const char EOF = '\u0003';

    public string Filename { get; }
    public SourceCode Source { get; }

    private Line _currLine;

    public char Current { get; private set; } = EOF;

    public bool EndOfStream
        => (_currLineIdx + 1 >= Source.RawLines.Length)
        && (_currColIdx + 1 >= _currLine.Length - 1);

    public LocationRange Position
        => new(_currLineIdx + 1, _currLineIdx + 1, _currColIdx + 1, _currColIdx + 1, Filename);

    object IEnumerator.Current => Current;

    private int _oldLineIdx = 0; // 0 so that it won't get updated the first time, which is crucial for empty files
    private int _currLineIdx = 0;
    private int _currColIdx = -1;

    private TextStream(TextStream other)
        : this(other.Source, other.Filename) { }

    public TextStream(string text, string filename)
        : this(new SourceCode(text.Split('\n').AsMemory()), filename) { }

    public TextStream(ImmutableArray<string> lines, string filename)
        : this(new SourceCode(lines), filename) { }

    public TextStream(SourceCode source, string filename) {
        Filename = filename;
        Source = source;

        // we need to set the first line for MoveNext/Back to work properly
        _currLine = new(Source.RawLines.Length == 0 ? "" : Source.RawLines.Span[0]);
    }

    public char ConsumeChar() {
        if (!MoveNext())
            return EOF;

        return Current;
    }

    public bool TryConsumeChar(out char result) {
        result = EOF;

        if (!MoveNext())
            return false;

        result = Current;
        return true;
    }

    // We don't use arrow-expr/getter props because they'd have to check for out-of-bounds.
    // This function is only called once we've started moving, and MoveNext/Back insures
    // we don't go out-of-bounds after init
    private void UpdateCurrent() {
        // don't create a new one each time
        if (_oldLineIdx != _currLineIdx) {
            _currLine = new(Source.RawLines.Span[_currLineIdx]);
            _oldLineIdx = _currLineIdx;
        }

        Current = _currLine[_currColIdx];
    }

    private bool MoveNext() {
        if (_currColIdx + 1 < _currLine.Length) {
            _currColIdx++;
            UpdateCurrent();
            return true;
        }

        if (_currLineIdx + 1 < Source.RawLines.Length) {
            _currColIdx = 0;
            _currLineIdx++;
            UpdateCurrent();
            return true;
        }

        return false;
    }

    private bool MoveBack() {
        if (_currColIdx - 1 >= 0) {
            _currColIdx--;
            UpdateCurrent();
            return true;
        }

        if (_currLineIdx - 1 >= 0) {
            _currLineIdx--;
            _currColIdx = Source.RawLines.Span[_currLineIdx].Length; // +1 for Line.Length, -1 for idx; cancels out
            UpdateCurrent();
            return true;
        }

        // if we're just at the beginning
        if (_currLineIdx == 0 && _currColIdx == 0)
            _currColIdx--;

        return false;
    }

    public void Reconsume() {
        bool canMoveBack = MoveBack();
        Debug.Assert(canMoveBack || (_currLineIdx == 0 && _currColIdx == -1));
    }

    public char PeekNextChar() {
        var output = ConsumeChar();
        Reconsume();
        return output;
    }

    internal TextStream Clone() => new(this);

    public IEnumerator<char> GetEnumerator() => this;
    IEnumerator IEnumerable.GetEnumerator() => this;

    bool IEnumerator.MoveNext() => MoveNext();
    public void Reset() => throw new NotImplementedException();
    public void Dispose() => throw new NotImplementedException();
}