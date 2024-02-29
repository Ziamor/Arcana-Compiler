using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ThisExpressionNode : PrimaryExpressionNode {
        public ExpressionNode Value { get; private set; }

        public ThisExpressionNode(ExpressionNode value) {
            this.Value = value;
        }


        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            return $"this: {Value}";
        }

        public override bool Equals(object? obj) {
            if (obj == null || !this.GetType().Equals(obj.GetType())) {
                return false;
            } else {
                ThisExpressionNode other = (ThisExpressionNode)obj;
                return this.Value.Equals(other.Value);
            }
        }

        public override int GetHashCode() {
            return HashCode.Combine(this.GetType(), Value.GetHashCode());
        }
    }
}
