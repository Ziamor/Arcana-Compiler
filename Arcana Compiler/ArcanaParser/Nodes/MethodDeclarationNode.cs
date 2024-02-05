using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class MethodDeclarationNode : ASTNode {
        public string MethodName { get; private set; }
        public string AccessModifier { get; private set; }
        public List<TypeNode> ReturnTypes { get; private set; }
        public List<ParameterNode> Parameters { get; private set; }
        public List<ASTNode> Body { get; private set; }

        public MethodDeclarationNode(string methodName, string? accessModifier, List<TypeNode> returnTypes, List<ParameterNode> parameters, List<ASTNode> body) {
            MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            AccessModifier = accessModifier ?? "private"; // Default to private if not specified
            ReturnTypes = returnTypes;
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString() {
            var parametersString = string.Join(", ", Parameters.Select(p => p.ToString()));
            var returnTypesString = ReturnTypes != null ? string.Join(", ", ReturnTypes.Select(rt => rt.ToString())) : "void";
            return $"{AccessModifier} func {MethodName}({parametersString}): {returnTypesString}";
        }
        public override bool Equals(object? obj) {
            if (obj is MethodDeclarationNode other) {
                return MethodName == other.MethodName &&
                       AccessModifier == other.AccessModifier &&
                       ReturnTypes.SequenceEqual(other.ReturnTypes) &&
                       Parameters.SequenceEqual(other.Parameters);
            }
            return false;
        }
        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 31 + MethodName.GetHashCode();
                hash = hash * 31 + AccessModifier.GetHashCode();
                foreach (var returnType in ReturnTypes) {
                    hash = hash * 31 + returnType.GetHashCode();
                }
                foreach (var param in Parameters) {
                    hash = hash * 31 + param.GetHashCode();
                }
                return hash;
            }
        }

    }

}
