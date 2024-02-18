using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class FieldModifierNode : ASTNode {
        public string Modifier { get; private set; }

        public FieldModifierNode(string modifier) {
            Modifier = modifier;
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            return Modifier;
        }

        public override bool Equals(object? obj) {
            return obj is FieldModifierNode otherNode && Modifier == otherNode.Modifier;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Modifier);
        }
    }
}
