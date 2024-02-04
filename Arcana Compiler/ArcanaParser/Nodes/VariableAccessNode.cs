using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    internal class VariableAccessNode : ASTNode {
        public QualifiedName QualifiedName { get; private set; }

        public VariableAccessNode(QualifiedName qualifiedName) {
            this.QualifiedName = qualifiedName ?? throw new ArgumentNullException(nameof(qualifiedName));
        }

        public string VariableName => QualifiedName.Identifier;

        public override string ToString() {
            return $"Variable: {QualifiedName}";
        }

        public override bool Equals(object? obj) {
            return obj is VariableAccessNode other &&
                   QualifiedName.Equals(other.QualifiedName);
        }

        public override int GetHashCode() {
            return QualifiedName.GetHashCode();
        }
    }
}