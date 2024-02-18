using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class VariableDeclarationNode : StatementNode {
        public TypeNode Type { get; private set; }
        public string Name { get; private set; }
        public ASTNode? InitialValue { get; private set; }

        public VariableDeclarationNode(TypeNode variableType, string variableName, ASTNode? initialValue = null) {
            Type = variableType;
            Name = variableName;
            InitialValue = initialValue;
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString() {
            string initialValueStr = InitialValue != null ? $" = {InitialValue}" : "";
            return $"{Type} {Name}{initialValueStr};";
        }

        public override bool Equals(object? obj) {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            VariableDeclarationNode otherNode = (VariableDeclarationNode)obj;
            return Type.Equals(otherNode.Type) &&
                   Name == otherNode.Name &&
                   Equals(InitialValue, otherNode.InitialValue);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Type, Name, InitialValue);
        }
    }
}