using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class Parameter {
        public string Name { get; }
        public IType Type { get; }

        public Parameter(string name, IType type) {
            Name = name;
            Type = type;
        }

        public override bool Equals(object? obj) {
            if (obj is Parameter other) {
                return Type.Equals(other.Type);
            }
            return false;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Type.GetHashCode());
        }
    }
}
