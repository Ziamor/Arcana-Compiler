using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ThisAssignmentNode : StatementNode {
        public IdentifierName IdentifierName { get; private set; }
        public ASTNode ValueExpression { get; private set; }

        public ThisAssignmentNode(IdentifierName identifierName, ASTNode valueExpression) {
            IdentifierName = identifierName;
            ValueExpression = valueExpression;
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            return $"this.{IdentifierName} = {ValueExpression}";
        }

        public override bool Equals(object? obj) {
            if (obj is ThisAssignmentNode otherNode)
                return IdentifierName == otherNode.IdentifierName && Equals(ValueExpression, otherNode.ValueExpression);
            return false;
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ IdentifierName.GetHashCode();
                hash = (hash * 16777619) ^ ValueExpression.GetHashCode();
                return hash;
            }
        }
    }
}
