using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol {
    internal class MethodSymbol : Symbol {
        public MethodSymbol(string name) : base(name) {
        }
        public override bool Equals(object? obj) {
            return obj is MethodSymbol other &&
                   base.Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode());
        }
    }
}
