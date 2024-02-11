namespace Arcana_Compiler.Common {
    public class IdentifierName {
        public static readonly IdentifierName DefaultNameSpace = new IdentifierName(new List<string> { "Default" });
        public List<string> Parts { get; private set; }

        public string Identifier => Parts.Last();
        public IdentifierName Qualifiers => new IdentifierName(Parts.Take(Parts.Count - 1).ToList());

        public bool IsFullyQualified => Parts.Count > 1;

        public IdentifierName(string name) {
            Parts = [name];
        }

        public IdentifierName(List<string> parts) {
            Parts = parts;
        }

        public static IdentifierName operator +(IdentifierName identifierName, string addition) {
            List<string> newParts = new List<string>(identifierName.Parts) { addition };
            return new IdentifierName(newParts);
        }

        public override string ToString() {
            return string.Join(".", Parts);
        }

        public override bool Equals(object? obj) {
            return obj is IdentifierName other &&
                   Parts.SequenceEqual(other.Parts);
        }

        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                foreach (var part in Parts) {
                    hash = hash * 31 + (part != null ? part.GetHashCode() : 0);
                }
                return hash;
            }
        }
    }

}
