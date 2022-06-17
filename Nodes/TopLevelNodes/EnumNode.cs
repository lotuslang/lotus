public record EnumNode(
    TypeDecName Name,
    IList<ValueNode> Values,
    Token EnumToken,
    Token OpenBracket,
    Token CloseBracket,
    bool IsValid
) : TopLevelNode(EnumToken, new LocationRange(EnumToken.Location, CloseBracket.Location), IsValid), IAccessible {
    public new static readonly EnumNode NULL
        = new(
            TypeDecName.NULL,
            Array.Empty<ValueNode>(),
            Token.NULL,
            Token.NULL,
            Token.NULL,
            false
        );

    public Token AccessKeyword { get; set; } = Token.NULL;

    private AccessLevel _accessLevel = AccessLevel.Unreachable;

    public AccessLevel GetAccessLevel() {
        if (_accessLevel != AccessLevel.Unreachable)
            return _accessLevel;

        if (AccessKeyword == Token.NULL) {
            _accessLevel = AccessLevel.Public;
        } else {
            var err =
                new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = AccessKeyword,
                    As = "an access modifier for a type",
                    Expected = "either 'public' or 'internal'"
                };

            switch (AccessKeyword.Representation) {
                case "public":
                    _accessLevel = AccessLevel.Public;
                    break;
                case "internal":
                    _accessLevel = AccessLevel.Internal;
                    break;
                case "private":
                    Logger.Error(err);
                    // TODO: IsValid = false;
                    _accessLevel = AccessLevel.Private;
                    break;
                case "internal protected": // internal protected isn't valid
                    Logger.Error(err);
                    // IsValid = false;
                    _accessLevel = AccessLevel.Protected;
                    break;
                default:
                    Logger.Error(err);
                    // IsValid = false;
                    _accessLevel = AccessLevel.Unreachable;
                    break;
            }
        }

        return _accessLevel;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}