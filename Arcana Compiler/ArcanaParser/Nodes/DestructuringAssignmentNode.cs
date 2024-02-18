using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class DestructuringAssignmentNode : StatementNode {
        public List<VariableDeclarationNode> VariableDeclarations { get; private set; }
        public ASTNode AssignedExpression { get; private set; }

        public DestructuringAssignmentNode(List<VariableDeclarationNode>? variableDeclarations, ASTNode? assignedExpression) {
            VariableDeclarations = variableDeclarations ?? throw new ArgumentNullException(nameof(variableDeclarations));
            AssignedExpression = assignedExpression ?? throw new ArgumentNullException(nameof(assignedExpression));
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            var variableNames = VariableDeclarations.Select(v => v.Name).Aggregate((i, j) => i + ", " + j);
            return $"{variableNames} = {AssignedExpression.ToString()}";
        }

        public override bool Equals(object? obj) {
            if (obj is DestructuringAssignmentNode other) {
                return VariableDeclarations.SequenceEqual(other.VariableDeclarations) &&
                       AssignedExpression.Equals(other.AssignedExpression);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                foreach (var decl in VariableDeclarations) {
                    hash = hash * 23 + decl.GetHashCode();
                }
                hash = hash * 23 + AssignedExpression.GetHashCode();
                return hash;
            }
        }
    }
}