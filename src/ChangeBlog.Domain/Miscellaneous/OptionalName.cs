namespace ChangeBlog.Domain.Common
{
    public record OptionalName
    {
        public const uint MaxLength = Name.MaxLength;

        public static readonly OptionalName Empty = new(string.Empty);

        private OptionalName(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static OptionalName Parse(string candidate)
        {
            if (string.IsNullOrWhiteSpace(candidate))
                return new OptionalName(string.Empty);

            var name = Name.Parse(candidate);
            return new OptionalName(name.Value);
        }

        public static bool TryParse(string candidate, out OptionalName name)
        {
            name = null;

            if (string.IsNullOrWhiteSpace(candidate))
            {
                name = new OptionalName(string.Empty);
                return true;
            }

            if (!Name.TryParse(candidate, out var n))
                return false;

            name = new OptionalName(n.Value);
            return true;
        }

        public static implicit operator string(OptionalName name) => name.Value;
    }
}
