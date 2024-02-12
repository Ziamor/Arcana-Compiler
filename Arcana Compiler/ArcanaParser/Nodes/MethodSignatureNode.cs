using Arcana_Compiler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class MethodSignatureNode : ASTNode {
        public List<TypeNode> ReturnTypes { get; }
        public string Name { get; }
        public List<ParameterNode> Parameters { get; }

        public MethodSignatureNode(string name, List<ParameterNode> parameters, List<TypeNode> returnTypes) {
            ReturnTypes = returnTypes;
            Name = name;
            Parameters = parameters;
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            var parameterTypes = string.Join(", ", Parameters.Select(p => p.ParameterType.ToString()));
            var returnTypes = string.Join(", ", ReturnTypes.Select(rt => rt.ToString()));
            return $"Method: {Name}({parameterTypes}) => {returnTypes}";
        }

        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (MethodSignatureNode)obj;
            return Name == other.Name &&
                   Enumerable.SequenceEqual(Parameters, other.Parameters) &&
                   Enumerable.SequenceEqual(ReturnTypes, other.ReturnTypes);
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Name.GetHashCode();
                hash = Parameters.Aggregate(hash, (current, param) => current * 23 + param.GetHashCode());
                hash = ReturnTypes.Aggregate(hash, (current, returnType) => current * 23 + returnType.GetHashCode());
                return hash;
            }
        }
    }
}
