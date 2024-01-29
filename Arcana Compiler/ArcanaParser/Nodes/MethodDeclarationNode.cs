namespace Arcana_Compiler.ArcanaParser.Nodes {
    internal class MethodDeclarationNode : ASTNode {
        public string MethodName { get; private set; }
        public string AccessModifier { get; private set; }
        public List<TypeNode> ReturnTypes { get; private set; }
        public List<ParameterNode> Parameters { get; private set; }
        public ASTNode Body { get; private set; } // Simplified representation for the method body for now

        public MethodDeclarationNode(string methodName, string accessModifier, List<TypeNode> returnTypes, List<ParameterNode> parameters, ASTNode body) {
            MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            AccessModifier = accessModifier ?? "private"; // Default to private if not specified
            ReturnTypes = returnTypes;
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            //Body = body ?? throw new ArgumentNullException(nameof(body));
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
