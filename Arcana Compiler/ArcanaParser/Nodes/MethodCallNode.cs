using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class MethodCallNode : ExpressionNode {
        public IdentifierName MethodName { get; private set; }
        public List<ASTNode> Arguments { get; private set; }

        public MethodCallNode(IdentifierName methodName, List<ASTNode> arguments) {
            MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString() {
            var argumentsString = string.Join(", ", Arguments.Select(arg => arg.ToString()));
            return $"{MethodName}({argumentsString})";
        }

        public override bool Equals(object? obj) {
            if (obj is MethodCallNode other) {
                return MethodName == other.MethodName &&
                       Arguments.SequenceEqual(other.Arguments);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = MethodName.GetHashCode();
                foreach (var arg in Arguments) {
                    hash = hash * 31 + arg.GetHashCode();
                }
                return hash;
            }
        }
    }
}
