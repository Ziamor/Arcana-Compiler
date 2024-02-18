using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ChainedPropertyAccessNode : ExpressionNode {
        public ASTNode PreviousNode { get; private set; }
        public string PropertyName { get; private set; }

        public ChainedPropertyAccessNode(ASTNode previousNode, string propertyName) {
            PreviousNode = previousNode ?? throw new ArgumentNullException(nameof(previousNode));
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString() {
            return $"{PreviousNode.ToString()}.{PropertyName}";
        }

        public override bool Equals(object? obj) {
            return obj is ChainedPropertyAccessNode other &&
                   EqualityComparer<ASTNode>.Default.Equals(PreviousNode, other.PreviousNode) &&
                   PropertyName == other.PropertyName;
        }

        public override int GetHashCode() {
            return HashCode.Combine(PreviousNode, PropertyName);
        }
    }
}