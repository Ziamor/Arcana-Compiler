using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    internal class ParameterNode : ASTNode {
        public TypeNode ParameterType { get; private set; }
        public string ParameterName { get; private set; }

        public ParameterNode(TypeNode parameterType, string parameterName) {
            ParameterType = parameterType ?? throw new ArgumentNullException(nameof(parameterType));
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        }

        public override string ToString() {
            return $"{ParameterType} {ParameterName}";
        }

        public override bool Equals(object? obj) {
            return obj is ParameterNode other &&
                   ParameterType == other.ParameterType &&
                   ParameterName == other.ParameterName;
        }

        public override int GetHashCode() {
            unchecked {
                return ParameterType.GetHashCode() * 31 + ParameterName.GetHashCode();
            }
        }
    }

}
