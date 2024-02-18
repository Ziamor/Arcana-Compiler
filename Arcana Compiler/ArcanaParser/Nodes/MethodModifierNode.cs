using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class MethodModifierNode : ASTNode {
        public string Modifier { get; private set; }

        public MethodModifierNode(string modifier) {
            Modifier = modifier;
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            return Modifier;
        }

        public override bool Equals(object? obj) {
            if (obj == null || obj.GetType() != GetType()) return false;

            MethodModifierNode other = (MethodModifierNode)obj;
            return Modifier == other.Modifier;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Modifier);
        }
    }
}
