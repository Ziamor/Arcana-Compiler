using Arcana_Compiler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class MethodSignatureNode : ASTNode {
        public string ReturnType { get; }
        public string Name { get; }
        public List<ParameterNode> Parameters { get; }

        public MethodSignatureNode(string returnType, string name, List<ParameterNode> parameters) {
            ReturnType = returnType;
            Name = name;
            Parameters = parameters;
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            var parameters = string.Join(", ", Parameters.Select(p => p.ToString()));
            return $"{ReturnType} {Name}({parameters})";
        }

        public override bool Equals(object? obj) {
            if (obj is MethodSignatureNode otherNode)
                return ReturnType == otherNode.ReturnType && Name == otherNode.Name && Parameters.SequenceEqual(otherNode.Parameters);
            return false;
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + ReturnType.GetHashCode();
                hash = hash * 23 + Name.GetHashCode();
                foreach (var parameter in Parameters) {
                    hash = hash * 23 + parameter.GetHashCode();
                }
                return hash;
            }
        }
    }
}
