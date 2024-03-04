using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ReturnStatementNode : StatementNode {
        public List<ExpressionNode> Expressions { get; private set; }

        public ReturnStatementNode(List<ExpressionNode> expressions) {
            Expressions = expressions ?? throw new ArgumentNullException(nameof(expressions));
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            var expressionsString = string.Join(", ", Expressions.Select(expr => expr.ToString()));
            return $"return {expressionsString};";
        }

        public override bool Equals(object? obj) {
            if (obj is ReturnStatementNode other) {
                return Expressions.SequenceEqual(other.Expressions);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = 19;
                foreach (var expr in Expressions) {
                    hash = hash * 31 + expr.GetHashCode();
                }
                return hash;
            }
        }
    }
}