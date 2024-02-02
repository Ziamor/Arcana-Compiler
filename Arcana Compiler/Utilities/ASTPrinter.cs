using Arcana_Compiler.ArcanaParser.Nodes;
using System;
using System.Text;

namespace Arcana_Compiler {
    public class ASTPrinter {
        public string Print(ASTNode node) {
            StringBuilder result = new StringBuilder();
            Print(node, result, "");
            return result.ToString();
        }

        private void Print(ASTNode node, StringBuilder result, string indent) {
            if (node == null) {
                return;
            }

            switch (node) {
                case ProgramNode programNode:
                    result.AppendLine(indent + "Program");
                    foreach (var classDecl in programNode.ClassDeclarations) {
                        Print(classDecl, result, indent + "  ");
                    }
                    break;
                case ClassDeclarationNode classNode:
                    result.AppendLine($"{indent}Class: {classNode.ClassName}");
                    foreach (var field in classNode.Fields) {
                        Print(field, result, indent + "  ");
                    }
                    foreach (var method in classNode.Methods) {
                        Print(method, result, indent + "  ");
                    }
                    break;
                case FieldDeclarationNode fieldNode:
                    result.AppendLine($"{indent}Field: {fieldNode.FieldName} ({fieldNode.FieldType})");
                    break;
                case MethodDeclarationNode methodNode:
                    result.AppendLine($"{indent}Method: {methodNode.MethodName}");
                    foreach (var param in methodNode.Parameters) {
                        Print(param, result, indent + "  ");
                    }
                    break;
                case ParameterNode parameterNode:
                    result.AppendLine($"{indent}Parameter: {parameterNode.ParameterName} ({parameterNode.ParameterType})");
                    break;
                default:
                    result.AppendLine(indent + "Unknown Node Type");
                    break;
            }
        }
    }
}
