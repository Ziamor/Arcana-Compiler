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
    }

    class ParameterTypeComparer : IEqualityComparer<Parameter> {
        public bool Equals(Parameter? x, Parameter? y) {
            return x?.Type == y?.Type;
        }

        public int GetHashCode(Parameter obj) {
            return obj.Type.GetHashCode();
        }
    }
}
