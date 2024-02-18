using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class NullLiteralNode : ExpressionNode {
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
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
