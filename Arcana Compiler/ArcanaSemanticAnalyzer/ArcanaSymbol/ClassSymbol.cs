using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol
{
    public class ClassSymbol : Symbol
    {
        public IdentifierName QualifiedName { get; }
        public ClassSymbol(string name, IdentifierName qualifiedName) : base(name)
        {
            QualifiedName = qualifiedName;
        }

        public override bool Equals(object? obj) {
            return obj is ClassSymbol other &&
                   base.Equals(other) &&
                   EqualityComparer<IdentifierName>.Default.Equals(QualifiedName, other.QualifiedName);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), QualifiedName);
        }
    }
}
