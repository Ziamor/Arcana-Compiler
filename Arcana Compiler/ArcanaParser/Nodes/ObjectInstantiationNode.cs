using Arcana_Compiler.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ObjectInstantiationNode : ASTNode {
        public QualifiedName QualifiedClassName { get; private set; }
        public List<ASTNode> ConstructorArguments { get; private set; }

        public ObjectInstantiationNode(QualifiedName className, List<ASTNode> constructorArguments) {
            QualifiedClassName = className ?? throw new ArgumentNullException(nameof(className));
            ConstructorArguments = constructorArguments ?? throw new ArgumentNullException(nameof(constructorArguments));
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            var argumentsString = string.Join(", ", ConstructorArguments.Select(arg => arg.ToString()));
            return $"new {QualifiedClassName}({argumentsString})";
        }

        public override bool Equals(object? obj) {
            if (obj is ObjectInstantiationNode other) {
                return QualifiedClassName.Equals(other.QualifiedClassName) &&
                       ConstructorArguments.SequenceEqual(other.ConstructorArguments);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = QualifiedClassName.GetHashCode();
                foreach (var arg in ConstructorArguments) {
                    hash = hash * 31 + arg.GetHashCode();
                }
                return hash;
            }
        }
    }
}
