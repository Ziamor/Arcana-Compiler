using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ThisExpressionNode : ExpressionNode {
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            return "this";
        }

        public override bool Equals(object? obj) {
            return obj is ThisExpressionNode;
        }

        public override int GetHashCode() {
            return typeof(ThisExpressionNode).GetHashCode();
        }
    }
}
