namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class UserType : IType {
        public string Name { get; private set; }

        public UserType(string name) {
            Name = name;
        }
        public override bool Equals(object? obj) {
            return obj is UserType other && Name == other.Name;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Name);
        }
    }

}
