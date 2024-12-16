using System.Collections;

namespace Lotus.Text;

public sealed class TextStream : ISourceCodeProvider, IEnumerable<char>, IEnumerator<char>
{
    [DebuggerDisplay("[l: {Length,nq}] {_str}")]
    private readonly struct Line {
        private readonly string _str;

        private readonly bool _shouldAddNewline;

        public readonly int Length => _str.Length + (_shouldAddNewline ? 1 : 0);
        public readonly char this[int idx] {
            get {
                if (idx < _str.Length) {
                    return _str[idx];
                } else {
                    if (_shouldAddNewline)
                        return '\n';
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public Line(string s, bool shouldAddNewline) {
            _str = s;
            _shouldAddNewline = shouldAddNewline;
        }
    }

    private const char EOF = '\u0003';

    public string Filename { get; }
    public SourceCode Source { get; }

    private Line _currLine;

    public char Current { get; private set; } = EOF;

    public bool EndOfStream { get; private set; }

    public LocationRange Position
        => new(_currLineIdx + 1, _currLineIdx + 1, _currColIdx + 1, _currColIdx + 1, Filename);

    object IEnumerator.Current => Current;

    private int _oldLineIdx = 0; // 0 so that it won't get updated the first time, which is crucial for empty files
    private int _currLineIdx = 0;
    private int _currColIdx = -1;

    private TextStream(TextStream other)
        : this(other.Source, other.Filename)
    {
        EndOfStream = other.EndOfStream;
        Current = other.Current;
        _oldLineIdx = other._oldLineIdx;
        _currLineIdx = other._currLineIdx;
        _currColIdx = other._currColIdx;
        _currLine = other._currLine;
    }

    public TextStream(string text, string filename)
        : this(new SourceCode(text.Split('\n').AsMemory()), filename) { }

    public TextStream(ImmutableArray<string> lines, string filename)
        : this(new SourceCode(lines), filename) { }

    // This line is at the end of the stream if:
    //      - it is beyond the last line
    //      - it is the last line AND it's empty
    private bool IsLineAtEndOfStream(int lineIdx)
        => Source.RawLines.Length == lineIdx
        || (Source.RawLines.Length == lineIdx + 1
            && Source.RawLines.Span[lineIdx].Length == 0);

    public TextStream(SourceCode source, string filename) {
        Filename = filename;
        Source = source;
        EndOfStream = IsLineAtEndOfStream(0);

        // we need to set the first line for MoveNext/Back to work properly
        _currLine = new(
            Source.RawLines.Length == 0 ? "" : Source.RawLines.Span[0],
            !EndOfStream
        );
    }

    public char ConsumeChar() {
        if (!MoveNext())
            return EOF;

        return Current;
    }

    public bool TryConsumeChar(out char result) {
        result = ConsumeChar();
        return !EndOfStream;
    }

    // We don't use arrow-expr/getter props because they'd have to check for out-of-bounds.
    // This function is only called once we've started moving, and MoveNext/Back insures
    // we don't go out-of-bounds after init
    private void UpdateCurrent() {
        // don't create a new one each time
        if (_oldLineIdx != _currLineIdx) {
            bool isLastLine = _currLineIdx + 1 == Source.RawLines.Length;
            _currLine = new(
                Source.RawLines.Span[_currLineIdx],
                shouldAddNewline: !isLastLine
            );
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

        if (!IsLineAtEndOfStream(_currLineIdx+1)) {
            _currColIdx = 0;
            _currLineIdx++;
            UpdateCurrent();
            return true;
        }

        EndOfStream = true;
        return false;
    }

    private bool MoveBack() {
        // in case we're at the end, we don't want to move the cursor, since
        // that would make the next call to Consume/MoveNext read the last
        // character again instead of an EOF
        if (EndOfStream) {
            UpdateCurrent();
            EndOfStream = false;
            return true;
        }

        if (_currColIdx - 1 >= 0) {
            _currColIdx--;
            UpdateCurrent();
            EndOfStream = false;
            return true;
        }

        if (_currLineIdx - 1 >= 0) {
            _currLineIdx--;
            _currColIdx = 0;
            UpdateCurrent(); // update to the right line first
            _currColIdx = _currLine.Length - 1;
            UpdateCurrent(); // then, go to the last character
            EndOfStream = false;
            return true;
        }

        // if we're just at the beginning
        if (_currLineIdx == 0 && _currColIdx == 0) {
            _currColIdx--;
            // in case the file is empty, we still need to set EndOfStream
            EndOfStream = _currLine.Length - 1 <= 0;
        }

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
    public void Dispose() {}
}