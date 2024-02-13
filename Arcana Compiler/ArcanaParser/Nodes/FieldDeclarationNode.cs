using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class FieldDeclarationNode : ASTNode
    {
        public List<FieldModifierNode> Modifiers { get; set; }
        public TypeNode FieldType { get; private set; }
        public string FieldName { get; private set; }
        public ASTNode? InitialValue { get; private set; } // Optional initial value

        public FieldDeclarationNode(TypeNode fieldType, string fieldName, List<FieldModifierNode> modifiers, ASTNode? initialValue = null)
        {
            FieldType = fieldType;
            FieldName = fieldName;
            InitialValue = initialValue;
            Modifiers = modifiers ?? new List<FieldModifierNode>();
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString()
        {
            string initialValueStr = InitialValue != null ? $" = {InitialValue}" : "";
            return $"{FieldType} {FieldName}{initialValueStr};";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj is FieldDeclarationNode otherNode)
            {
                return FieldType == otherNode.FieldType &&
                       FieldName == otherNode.FieldName &&
                       Equals(InitialValue, otherNode.InitialValue); // Recursive equality check for AST nodes
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FieldType, FieldName, InitialValue);
        }
    }
}
