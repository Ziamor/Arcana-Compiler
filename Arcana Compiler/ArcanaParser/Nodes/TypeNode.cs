using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    internal class TypeNode : ASTNode {
        public string TypeName { get; private set; }
        public bool IsNullable { get; private set; }

        public TypeNode(string typeName, bool isNullable) {
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
            IsNullable = isNullable;
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
