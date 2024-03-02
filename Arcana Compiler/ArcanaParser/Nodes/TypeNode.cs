using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class TypeNode : ASTNode {
        public string TypeName { get; private set; }
        public bool IsNullable { get; private set; }
        public bool IsArray { get; }
        public int ArrayDimensions { get; }

        public TypeNode(string typeName, bool isNullable, bool isArray, int arrayDimensions) {
            TypeName = typeName;
            IsNullable = isNullable;
            IsArray = isArray;
            ArrayDimensions = arrayDimensions;
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString() {
            var arraySuffix = IsArray ? new string('[', ArrayDimensions) + new string(']', ArrayDimensions) : "";
            return $"{TypeName}{arraySuffix}{(IsNullable ? "?" : "")}";
        }

        public override bool Equals(object? obj) {
            return obj is TypeNode other &&
                   TypeName == other.TypeName &&
                   IsNullable == other.IsNullable &&
                   IsArray == other.IsArray &&
                   ArrayDimensions == other.ArrayDimensions;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 31 + TypeName.GetHashCode();
                hash = hash * 31 + IsNullable.GetHashCode();
                hash = hash * 31 + IsArray.GetHashCode();
                hash = hash * 31 + ArrayDimensions.GetHashCode();
                return hash;
            }
        }
    }
}
