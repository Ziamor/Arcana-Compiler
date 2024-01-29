using System.Text;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    internal class ClassDeclarationNode : ASTNode {
        public string ClassName { get; private set; }
        public List<ParentTypeNode> ParentTypes { get; private set; } = new List<ParentTypeNode>();
        public List<FieldDeclarationNode> Fields { get; private set; } = new List<FieldDeclarationNode>();
        public List<MethodDeclarationNode> Methods { get; private set; } = new List<MethodDeclarationNode>();

        public ClassDeclarationNode(string className, List<ParentTypeNode> parentTypes, List<FieldDeclarationNode> fields, List<MethodDeclarationNode> methods) {
            ClassName = className;
            ParentTypes = parentTypes;
            Fields = fields;
            Methods = methods;
        }

        public override string ToString() {
            var builder = new StringBuilder();
            builder.AppendLine($"Class: {ClassName}");

            if (ParentTypes.Any()) {
                var parentTypesString = string.Join(", ", ParentTypes.Select(p => p.TypeName));
                builder.AppendLine($"Inherits/Implements: {parentTypesString}");
            }

            builder.AppendLine("Fields:");
            foreach (var field in Fields) {
                builder.AppendLine(field.ToString());
            }

            builder.AppendLine("Methods:");
            foreach (var method in Methods) {
                builder.AppendLine(method.ToString());
            }
            return builder.ToString();
        }
        public override bool Equals(object? obj) {
            return obj is ClassDeclarationNode other &&
                   ClassName == other.ClassName &&
                   ParentTypes.SequenceEqual(other.ParentTypes) &&
                   Fields.SequenceEqual(other.Fields);
        }
        public override int GetHashCode() {
            unchecked {
                int hash = ClassName.GetHashCode();
                foreach (var parentType in ParentTypes) {
                    hash = hash * 31 + parentType.GetHashCode();
                }
                foreach (var field in Fields) {
                    hash = hash * 31 + field.GetHashCode();
                }
                foreach (var method in Methods) {
                    hash = hash * 31 + method.GetHashCode();
                }
                return hash;
            }
        }
    }
}

