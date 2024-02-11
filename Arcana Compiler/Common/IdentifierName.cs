namespace Arcana_Compiler.Common {
    public class IdentifierName {
        public static readonly IdentifierName DefaultNameSpace = new IdentifierName(new List<string> { "Default" });
        public List<string> AccessPath { get; private set; }

        public string Identifier => AccessPath.Last();
        public IdentifierName Qualifiers => new IdentifierName(AccessPath.Take(AccessPath.Count - 1).ToList());

        public bool IsFullyQualified => AccessPath.Count > 1;

        public IdentifierName(string name) {
            AccessPath = [name];
        }

        public IdentifierName(List<string> accessPath) {
            AccessPath = accessPath;
        }

        public static IdentifierName operator +(IdentifierName identifierName, string addition) {
            List<string> newParts = new List<string>(identifierName.AccessPath) { addition };
            return new IdentifierName(newParts);
        }

        public override string ToString() {
            return string.Join(".", AccessPath);
        }

        public override bool Equals(object? obj) {
            return obj is IdentifierName other &&
                   AccessPath.SequenceEqual(other.AccessPath);
        }

        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                foreach (var part in AccessPath) {
                    hash = hash * 31 + (part != null ? part.GetHashCode() : 0);
                }
                return hash;
            }
        }
    }

}
