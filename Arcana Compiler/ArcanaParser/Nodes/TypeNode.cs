using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class TypeNode : ASTNode {
        public string TypeName { get; private set; }
        public bool IsNullable { get; private set; }

        public TypeNode(string typeName, bool isNullable) {
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
            IsNullable = isNullable;
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString() {
            return $"{TypeName}{(IsNullable ? "?" : "")}";
        }

        public override bool Equals(object? obj) {
            return obj is TypeNode other &&
                   TypeName == other.TypeName &&
                   IsNullable == other.IsNullable;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 31 + TypeName.GetHashCode();
                hash = hash * 31 + IsNullable.GetHashCode();
                return hash;
            }
        }
    }
}
