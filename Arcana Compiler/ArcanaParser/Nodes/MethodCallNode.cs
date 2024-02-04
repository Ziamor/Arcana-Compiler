using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    internal class MethodCallNode : ASTNode {
        public string MethodName { get; private set; }
        public List<ASTNode> Arguments { get; private set; }

        public MethodCallNode(string methodName, List<ASTNode> arguments) {
            MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        public override string ToString() {
            var argumentsString = string.Join(", ", Arguments.Select(arg => arg.ToString()));
            return $"{MethodName}({argumentsString})";
        }

        public override bool Equals(object? obj) {
            if (obj is MethodCallNode other) {
                return MethodName == other.MethodName &&
                       Arguments.SequenceEqual(other.Arguments);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = MethodName.GetHashCode();
                foreach (var arg in Arguments) {
                    hash = hash * 31 + arg.GetHashCode();
                }
                return hash;
            }
        }
    }
}
