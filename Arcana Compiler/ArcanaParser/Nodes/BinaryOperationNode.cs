using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class BinaryOperationNode : ASTNode {
        public ASTNode Left { get; private set; }
        public Token Operator { get; private set; }
        public ASTNode Right { get; private set; }

        public BinaryOperationNode(ASTNode left, Token operatorToken, ASTNode right) {
            Left = left;
            Operator = operatorToken;
            Right = right;
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString() {
            return $"Binary Operation: {Left} {Operator.Value} {Right}";
        }

        public override bool Equals(object? obj) {
            if (obj is BinaryOperationNode other) {
                return Left.Equals(other.Left) && Operator.Equals(other.Operator) && Right.Equals(other.Right);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked { // Overflow is fine, just wrap
                int hash = 17;
                hash = hash * 23 + Left.GetHashCode();
                hash = hash * 23 + Operator.GetHashCode();
                hash = hash * 23 + Right.GetHashCode();
                return hash;
            }
        }
    }
}
