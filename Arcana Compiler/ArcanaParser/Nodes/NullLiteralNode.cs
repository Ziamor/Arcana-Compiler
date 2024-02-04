namespace Arcana_Compiler.ArcanaParser.Nodes {
    internal class NullLiteralNode : ASTNode {
        public override string ToString() {
            return "null";
        }

        public override bool Equals(object? obj) {
            return obj is NullLiteralNode;
        }

        public override int GetHashCode() {
            return typeof(NullLiteralNode).GetHashCode();
        }
    }

}
