using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class VariableAssignmentNode : StatementNode {
        public string VariableName { get; private set; }
        public ASTNode AssignedExpression { get; private set; }

        public VariableAssignmentNode(string variableName, ASTNode assignedExpression) {
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
            AssignedExpression = assignedExpression ?? throw new ArgumentNullException(nameof(assignedExpression));
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString() {
            return $"{VariableName} = {AssignedExpression.ToString()}";
        }

        public override bool Equals(object? obj) {
            if (obj is VariableAssignmentNode other) {
                return VariableName == other.VariableName &&
                       AssignedExpression.Equals(other.AssignedExpression);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 23 + VariableName.GetHashCode();
                hash = hash * 23 + AssignedExpression.GetHashCode();
                return hash;
            }
        }
    }
}
