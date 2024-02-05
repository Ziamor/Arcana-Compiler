using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class ParameterNode : ASTNode {
        public TypeNode ParameterType { get; private set; }
        public string ParameterName { get; private set; }

        public ParameterNode(TypeNode parameterType, string parameterName) {
            ParameterType = parameterType ?? throw new ArgumentNullException(nameof(parameterType));
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
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
