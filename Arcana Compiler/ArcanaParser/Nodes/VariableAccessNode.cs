using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class VariableAccessNode : ExpressionNode {
        public IdentifierName QualifiedName { get; private set; }

        public VariableAccessNode(IdentifierName qualifiedName) {
            this.QualifiedName = qualifiedName ?? throw new ArgumentNullException(nameof(qualifiedName));
        }

        public string VariableName => QualifiedName.Identifier;
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
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