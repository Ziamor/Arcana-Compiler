namespace Arcana_Compiler.ArcanaParser.Nodes
{
    internal class FieldDeclarationNode : ASTNode
    {
        public TypeNode FieldType { get; private set; }
        public string FieldName { get; private set; }
        public ASTNode? InitialValue { get; private set; } // Optional initial value

        public FieldDeclarationNode(TypeNode fieldType, string fieldName, ASTNode? initialValue = null)
        {
            FieldType = fieldType;
            FieldName = fieldName;
            InitialValue = initialValue;
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
