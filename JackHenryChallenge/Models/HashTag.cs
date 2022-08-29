namespace JackHenryChallenge.Entities
{
    /// <summary>
    /// Twitter Hash Tag
    /// </summary>
    /// <seealso cref="System.IEquatable&lt;JackHenryChallenge.Entities.HashTag&gt;" />
    public class HashTag : IEquatable<HashTag?>
    {
        public string Tag { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            return Equals(obj as HashTag);
        }

        public bool Equals(HashTag? other)
        {
            return other is not null &&
                   Tag.Equals(other.Tag, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Tag.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return Tag;
        }
    }
}