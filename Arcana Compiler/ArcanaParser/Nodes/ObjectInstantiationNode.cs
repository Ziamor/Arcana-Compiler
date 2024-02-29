using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ObjectInstantiationNode : PrimaryExpressionNode {
        public IdentifierName ClassName { get; private set; }
        public List<ASTNode> ConstructorArguments { get; private set; }

        public ObjectInstantiationNode(IdentifierName className, List<ASTNode> constructorArguments) {
            ClassName = className ?? throw new ArgumentNullException(nameof(className));
            ConstructorArguments = constructorArguments ?? throw new ArgumentNullException(nameof(constructorArguments));
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            var argumentsString = string.Join(", ", ConstructorArguments.Select(arg => arg.ToString()));
            return $"new {ClassName}({argumentsString})";
        }

        public override bool Equals(object? obj) {
            if (obj is ObjectInstantiationNode other) {
                return ClassName.Equals(other.ClassName) &&
                       ConstructorArguments.SequenceEqual(other.ConstructorArguments);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = ClassName.GetHashCode();
                foreach (var arg in ConstructorArguments) {
                    hash = hash * 31 + arg.GetHashCode();
                }
                return hash;
            }
        }
    }
}
