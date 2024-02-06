namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol {
    public class MethodSymbol : Symbol {
        public Signature Signature { get; }

        public MethodSymbol(string name, Signature signature)
            : base(name) {
            Signature = signature;
        }

        public override bool Equals(object? obj) {
            return obj is MethodSymbol other &&
                   base.Equals(other) &&
                   Signature.Equals(other.Signature);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Signature.GetHashCode());
        }
    }
}