namespace Lotus.Text;

public interface ISourceCodeProvider {
    string Filename { get; }

    SourceCode Source { get; }
}