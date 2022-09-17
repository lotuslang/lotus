internal class DeterministicStringComparer : IEqualityComparer<string>
{
    public static readonly DeterministicStringComparer Instance = new();

    public bool Equals(string? s1, string? s2) => GetHashCode(s1!) == GetHashCode(s2!);

    public int GetHashCode(string str) {
        unchecked {
            int hash1 = 5381;
            int hash2 = hash1;

            for (int i = 0; i < str.Length && str[i] != '\0'; i += 2) {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1 || str[i + 1] == '\0')
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }
}