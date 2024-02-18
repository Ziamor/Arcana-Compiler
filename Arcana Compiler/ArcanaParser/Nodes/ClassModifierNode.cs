using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ClassModifierNode : ASTNode {
        public string Modifier { get; private set; }

        public ClassModifierNode(string modifier) {
            Modifier = modifier;
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            return Modifier;
        }

        public override bool Equals(object? obj) {
            if (obj == null) return false;

            if (obj is ClassModifierNode otherNode) {
                return Modifier == otherNode.Modifier;
            }
            return false;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Modifier);
        }
    }
}
