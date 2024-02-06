using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class ReturnType {
        public IType Type { get; }

        public ReturnType(IType type) {
            Type = type;
        }
    }
}
