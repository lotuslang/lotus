using System.Text;

namespace Lotus.Utils;

// yes, I know IndentedTextWriter, in fact that's the reason I
// wrote this, because the BCL's version has a pretty bad API
// compared to StringBuilder. Initially, I planned to use a
// combination of StringBuilder/Writer+IndentedTextWriter, by
// only using the latter for newlines, which was actually quite
// cumbersome, but I soon discovered that IndentedTextWriter only
// adds tabs *after* you've written something to the line, i.e.
// WriteLine() *queues up* tabs, but they only get appended in
// the next call. This wouldn't be a problem if I was *only* using
// the writer directly, but I had to call the underlying StringBuilder
// in a few places, which made the whole state completely inconsistent.
//
// With this class, I don't need to separate calls to the writer
// and the strBuilder AND it takes care of everything for me.
//
// Once thing you'll notice is that I kept the "flush tabs later"
// mechanism of the original IndentedTextWriter. If you were thinking
// "hmm, there must be a reason it worked like that," it's because,
// if you append tabs in the AppendLine call, then decrease the indent
// level, the next write will have the old level regardless. By only
// flushing tabs when writing non-newlines, we don't need to worry
// about what the previous indent level was when writing!
[DebuggerDisplay("{ToString(),nq}")]
public sealed class IndentedStringBuilder
{
    public StringBuilder InternalStringBuilder => _sb;
    private readonly StringBuilder _sb;

    public int Indent { get; set; } = 0;

    private bool _tabsPending = false;

    private const char _tab = '\t';

    public IndentedStringBuilder() : this(new StringBuilder()) {}

    public IndentedStringBuilder(StringBuilder sb) => _sb = sb;

    private void FlushTabs() {
        if (_tabsPending) {
            _sb.Append(new string(_tab, Indent));
            _tabsPending = false;
        }
    }

    public IndentedStringBuilder Append(char c) {
        FlushTabs();
        _sb.Append(c);
        return this;
    }

    public IndentedStringBuilder Append(object o) {
        FlushTabs();
        _sb.Append(o);
        return this;
    }

    public IndentedStringBuilder Append(string s) {
        FlushTabs();
        _sb.Append(s);
        return this;
    }

    public IndentedStringBuilder AppendLine() {
        _sb.AppendLine();
        _tabsPending = true;
        return this;
    }

    public IndentedStringBuilder AppendLine(string s) {
        FlushTabs();
        _sb.AppendLine(s);
        _tabsPending = true;
        return this;
    }

    public IndentedStringBuilder AppendLine(char c) {
        FlushTabs();
        _sb.Append(c);
        return AppendLine();
    }

    public IndentedStringBuilder AppendLine(object o) {
        FlushTabs();
        _sb.Append(o);
        return AppendLine();
    }

    public IndentedStringBuilder AppendLineNoIndent(string s) {
        _sb.AppendLine(s);
        return this;
    }

    public IndentedStringBuilder AppendLineNoIndent(char c) {
        _sb.Append(c).AppendLine();
        return this;
    }

    public IndentedStringBuilder AppendLineNoIndent(object o) {
        _sb.Append(o).AppendLine();
        return this;
    }

    public override string ToString() => _sb.ToString();
}