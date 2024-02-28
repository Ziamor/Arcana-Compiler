using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public enum UnaryOperatorPosition {
        Prefix,
        Postfix
    }

    public class UnaryOperationNode : ExpressionNode {
        public Token Operator { get; private set; }
        public ExpressionNode Operand { get; private set; }
        public UnaryOperatorPosition UnaryOperatorPosition { get; private set; }

        public UnaryOperationNode(Token operatorToken, ExpressionNode operand, UnaryOperatorPosition unaryOperatorPosition) {
            Operator = operatorToken;
            Operand = operand;
            UnaryOperatorPosition = unaryOperatorPosition;
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString() {
            switch (UnaryOperatorPosition) {
                case UnaryOperatorPosition.Prefix:
                    return $"Unary Operation: ({Operator.Value} {Operand})";
                case UnaryOperatorPosition.Postfix:
                    return $"Unary Operation: ({Operand} {Operator.Value})";
                default:
                    throw new InvalidOperationException("Unknown unary operator position.");
            }
        }

        public override bool Equals(object? obj) {
            if (obj is UnaryOperationNode other) {
                return Operator.Equals(other.Operator) && Operand.Equals(other.Operand);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked { // Overflow is fine, just wrap
                int hash = 17;
                hash = hash * 23 + Operator.GetHashCode();
                hash = hash * 23 + Operand.GetHashCode();
                return hash;
            }
        }
    }
}
