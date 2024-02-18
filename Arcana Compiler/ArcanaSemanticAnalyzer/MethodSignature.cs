using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class MethodSignature {
        public string MethodName { get; }
        // Store type names as strings initially
        public List<string> ParameterTypesNames { get; }
        public List<string> ReturnTypesNames { get; }

        // Resolved TypeSymbols for parameters and return types
        public List<TypeSymbol> ParameterTypes { get; private set; }
        public List<TypeSymbol> ReturnTypes { get; private set; }

        public MethodSignature(string methodName, List<string> parameterTypeNames, List<string> returnTypeNames) {
            MethodName = methodName;
            ParameterTypesNames = parameterTypeNames;
            ReturnTypesNames = returnTypeNames;
            ParameterTypes = new List<TypeSymbol>();
            ReturnTypes = new List<TypeSymbol>();
        }

        public void ResolveTypes(SymbolTable symbolTable) {
            throw new NotImplementedException();
        }

        public override bool Equals(object? obj) {
            if (obj is MethodSignature other) {
                return MethodName == other.MethodName &&
                       ParameterTypesNames.SequenceEqual(other.ParameterTypesNames) &&
                       ReturnTypesNames.SequenceEqual(other.ReturnTypesNames);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 23 + MethodName.GetHashCode();
                hash = ParameterTypesNames.Aggregate(hash, (h, type) => h * 23 + type.GetHashCode());
                hash = ReturnTypesNames.Aggregate(hash, (h, type) => h * 23 + type.GetHashCode());
                return hash;
            }
        }
    }
}
