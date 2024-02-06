using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class BuiltInType : IType {
        public string Name { get; private set; }

        private BuiltInType(string name) { Name = name; }

        public static readonly BuiltInType Int = new BuiltInType("int");
        public static readonly BuiltInType Float = new BuiltInType("float");
        public override bool Equals(object? obj) {
            return obj is BuiltInType other && Name == other.Name;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Name);
        }
    }

}
