﻿using Arcana_Compiler.Common;
using System.Text;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class ClassDeclarationNode : ASTNode {
        public IdentifierName QualifiedClassName { get; private set; }
        public string ClassName { get { return QualifiedClassName.Identifier; } }
        public IdentifierName Namespace { get { return QualifiedClassName.Qualifiers; } }
        public string AccessModifier { get; private set; }
        public List<ClassModifierNode> ClassModifierNodes { get; private set; }
        public List<ParentTypeNode> ParentTypes { get; private set; } = new List<ParentTypeNode>();
        public List<FieldDeclarationNode> Fields { get; private set; } = new List<FieldDeclarationNode>();
        public List<MethodDeclarationNode> Methods { get; private set; } = new List<MethodDeclarationNode>();
        public List<ClassDeclarationNode> NestedClasses { get; internal set; }

        public ClassDeclarationNode(IdentifierName qualifiedClassName, string? accessModifier, List<ClassModifierNode> classModifierNodes, List<ParentTypeNode> parentTypes, List<FieldDeclarationNode> fields, List<MethodDeclarationNode> methods, List<ClassDeclarationNode> nestedClasses) {
            QualifiedClassName = qualifiedClassName;
            AccessModifier = accessModifier ?? "private";
            ClassModifierNodes = classModifierNodes;
            ParentTypes = parentTypes;
            Fields = fields;
            Methods = methods;
            NestedClasses = nestedClasses;
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString() {
            var builder = new StringBuilder();
            if (Namespace != null) {
                builder.AppendLine($"Namespace: {Namespace}");
            }
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
                   ((Namespace == null && other.Namespace == null) || (Namespace?.Equals(other.Namespace) == true)) &&
                   ParentTypes.SequenceEqual(other.ParentTypes) &&
                   Fields.SequenceEqual(other.Fields) &&
                   Methods.SequenceEqual(other.Methods);
        }
        public override int GetHashCode() {
            unchecked {
                int hash = ClassName.GetHashCode();
                hash = hash * 31 + (Namespace != null ? Namespace.GetHashCode() : 0);

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

