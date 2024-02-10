﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol {
    public class VariableSymbol : Symbol {
        public VariableSymbol(string name) : base(name) {
        }

        public override bool Equals(object? obj) {
            return obj is FieldSymbol other &&
                   base.Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode());
        }
    }
}