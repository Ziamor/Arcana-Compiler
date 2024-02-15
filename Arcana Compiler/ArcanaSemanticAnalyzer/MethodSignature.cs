using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class MethodSignature {
        public string MethodName { get; }
        public List<string> ParameterTypes { get; }
        public List<string> ReturnTypes { get; }

        public MethodSignature(string methodName, List<string> parameterTypes, List<string> returnTypes) {
            MethodName = methodName;
            ParameterTypes = parameterTypes;
            ReturnTypes = returnTypes;
        }
        public override bool Equals(object? obj) {
            if (obj is MethodSignature other) {
                return MethodName == other.MethodName &&
                       ParameterTypes.SequenceEqual(other.ParameterTypes) &&
                       ReturnTypes.SequenceEqual(other.ReturnTypes);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 23 + MethodName.GetHashCode();
                hash = ParameterTypes.Aggregate(hash, (h, type) => h * 23 + type.GetHashCode());
                hash = ReturnTypes.Aggregate(hash, (h, type) => h * 23 + type.GetHashCode());
                return hash;
            }
        }
    }
}
