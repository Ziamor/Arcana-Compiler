using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol {
    public class MethodSymbol : Symbol {
        public List<Parameter> Parameters { get; }
        public List<ReturnType> ReturnTypes { get; }

        public MethodSymbol(string name, List<Parameter> parameters, List<ReturnType> returnTypes)
            : base(name) {
            Parameters = parameters;
            ReturnTypes = returnTypes;
        }

        public override bool Equals(object? obj) {
            return obj is MethodSymbol other &&
                   base.Equals(other) &&
                   Enumerable.SequenceEqual(Parameters, other.Parameters) &&
                   Enumerable.SequenceEqual(ReturnTypes, other.ReturnTypes);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Parameters, ReturnTypes);
        }
    }
}
