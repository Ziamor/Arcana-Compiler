namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol {
    public class FieldSymbol : Symbol {
        public FieldSymbol(string name) : base(name) {
        }
        public override bool Equals(object? obj) {
            return obj is FieldSymbol other &&
                   base.Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode());
        }
    }
}
