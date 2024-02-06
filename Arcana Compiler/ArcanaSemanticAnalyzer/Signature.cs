namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class Signature {
        public IReadOnlyList<Parameter> Parameters { get; }
        public IReadOnlyList<ReturnType> ReturnTypes { get; }

        public Signature(IEnumerable<Parameter> parameters, IEnumerable<ReturnType> returnTypes) {
            Parameters = parameters.ToList();
            ReturnTypes = returnTypes.ToList();
        }

        public override bool Equals(object? obj) {
            if (obj is Signature other) {
                return Enumerable.SequenceEqual(Parameters, other.Parameters) &&
                       Enumerable.SequenceEqual(ReturnTypes, other.ReturnTypes);
            }
            return false;
        }


        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                foreach (var param in Parameters) {
                    hash = hash * 23 + param.GetHashCode();
                }
                foreach (var returnType in ReturnTypes) {
                    hash = hash * 23 + returnType.GetHashCode();
                }
                return hash;
            }
        }


        public override string ToString() {
            var parameterTypes = string.Join(",", Parameters.Select(p => p.Type.Name));
            var returnTypes = string.Join(",", ReturnTypes.Select(r => r.Type.Name));
            return $"{parameterTypes}->{returnTypes}";
        }
    }
}
