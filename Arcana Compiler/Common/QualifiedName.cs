namespace Arcana_Compiler.Common {
    public class QualifiedName {
        public static readonly QualifiedName Default = new QualifiedName(new List<string> { "Default" });
        public List<string> Parts { get; private set; }

        public string Identifier => Parts.Last();
        public QualifiedName NamespacePart => new QualifiedName(Parts.Take(Parts.Count - 1).ToList());

        public QualifiedName(List<string> parts) {
            Parts = parts;
        }

        public override string ToString() {
            return string.Join(".", Parts);
        }

        public override bool Equals(object? obj) {
            return obj is QualifiedName other &&
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
