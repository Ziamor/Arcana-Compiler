using Arcana_Compiler.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ArrayAccessNode : ExpressionNode {
        public ExpressionNode Array { get; private set; }
        public ExpressionNode Index { get; private set; }

        public ArrayAccessNode(ExpressionNode array, ExpressionNode index) {
            Array = array ?? throw new ArgumentNullException(nameof(array));
            Index = index ?? throw new ArgumentNullException(nameof(index));
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            return $"{Array}[{Index}]";
        }

        public override bool Equals(object? obj) {
            if (obj is ArrayAccessNode other) {
                return Array.Equals(other.Array) && Index.Equals(other.Index);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = Array.GetHashCode();
                hash = hash * 31 + Index.GetHashCode();
                return hash;
            }
        }
    }
}