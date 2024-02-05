using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class UnaryOperationNode : ASTNode {
        public Token Operator { get; private set; }
        public ASTNode Operand { get; private set; }

        public UnaryOperationNode(Token operatorToken, ASTNode operand) {
            Operator = operatorToken;
            Operand = operand;
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString() {
            return $"Unary Operation: {Operator.Value} {Operand}";
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
