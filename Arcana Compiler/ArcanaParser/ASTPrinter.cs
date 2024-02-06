using Arcana_Compiler.ArcanaParser.Nodes;
using System.Text;

namespace Arcana_Compiler.ArcanaParser {
    public class ASTPrinter
    {
        public string Print(ASTNode node)
        {
            StringBuilder result = new StringBuilder();
            Print(node, result, "");
            return result.ToString();
        }

        private string PrintExpression(ASTNode expression) {
            switch (expression) {
                case LiteralNode literalNode:
                    return literalNode.Value.ToString() ?? "EMPTY";
                case VariableAccessNode variableNode:
                    return variableNode.QualifiedName.ToString();
                case ChainedMethodCallNode chainedMethodCallNode:
                    return $"{PrintExpression(chainedMethodCallNode.PreviousNode)} -> {chainedMethodCallNode.CurrentCall}";
                default:
                    return expression.ToString() ?? "EMPTY";
            }
        }

        private void Print(ASTNode node, StringBuilder result, string indent)
        {
            if (node == null)
            {
                return;
            }

            switch (node)
            {
                case ProgramNode programNode:
                    result.AppendLine(indent + "Program");
                    foreach (var import in programNode.Imports) {
                        Print(import, result, indent + "  ");
                    }
                    foreach (var classDecl in programNode.ClassDeclarations)
                    {
                        Print(classDecl, result, indent + "  ");
                    }
                    break;
                case ImportDeclarationNode importDeclarationNode:
                    result.AppendLine($"{indent}Import: {importDeclarationNode.ImportedNamespace}");
                    break;
                case ClassDeclarationNode classNode:
                    result.AppendLine($"{indent}Class: {classNode.ClassName} Namespace: {classNode.Namespace}");
                    foreach (var field in classNode.Fields)
                    {
                        Print(field, result, indent + "  ");
                    }
                    foreach (var method in classNode.Methods)
                    {
                        Print(method, result, indent + "  ");
                    }
                    break;
                case FieldDeclarationNode fieldNode:
                    result.AppendLine($"{indent}Field Decl: {fieldNode.FieldName} ({fieldNode.FieldType})");
                    break;
                case MethodDeclarationNode methodNode:
                    result.AppendLine($"{indent}Method: {methodNode.MethodName}");
                    foreach (var param in methodNode.Parameters)
                    {
                        Print(param, result, indent + "  ");
                    }

                    if (methodNode.Body.Count > 0)
                    {
                        foreach (var param in methodNode.Body)
                        {
                            Print(param, result, indent + "  ");
                        }
                    }
                    else
                    {
                        result.AppendLine(indent + indent + "Empty");
                    }
                    break;
                case ParameterNode parameterNode:
                    result.AppendLine($"{indent}Parameter: {parameterNode.ParameterName} ({parameterNode.ParameterType})");
                    break;
                case MethodCallNode methodCallNode:
                    result.AppendLine($"{indent}Method Call: {methodCallNode.MethodName}");
                    foreach (var argument in methodCallNode.Arguments)
                    {
                        Print(argument, result, indent + "  ");
                    }
                    break;
                
                case LiteralNode literalNode:
                    result.AppendLine($"{indent}Literal: {literalNode}");
                    break;
                case VariableAccessNode variableAccessNode:
                    result.AppendLine($"{indent}Variable: {variableAccessNode.QualifiedName}");
                    break;
                case VariableDeclarationNode variableDeclarationNode:
                    string initialValueOutput = variableDeclarationNode.InitialValue != null
                        ? $" = {PrintExpression(variableDeclarationNode.InitialValue)}"
                        : string.Empty;

                    result.AppendLine($"{indent}Variable Decl: {variableDeclarationNode.Name} ({variableDeclarationNode.Type}){initialValueOutput}");
                    break;
                case VariableAssignmentNode variableAssignmentNode:
                    result.AppendLine($"{indent}Variable: {variableAssignmentNode.VariableName} = {PrintExpression(variableAssignmentNode.AssignedExpression)}");
                    break;
                case IfStatementNode ifStatementNode:
                    foreach (var (Condition, Statements) in ifStatementNode.ConditionsAndStatements)
                    {
                        result.AppendLine(indent + "If Condition:");
                        Print(Condition, result, indent + "  ");
                        foreach (var statement in Statements)
                        {
                            Print(statement, result, indent + "    ");
                        }
                    }
                    if (ifStatementNode.ElseStatements != null)
                    {
                        result.AppendLine(indent + "Else:");
                        foreach (var elseStatement in ifStatementNode.ElseStatements)
                        {
                            Print(elseStatement, result, indent + "  ");
                        }
                    }
                    break;
                default:
                    result.AppendLine(indent + "Unknown Node Type");
                    break;
            }
        }
    }
}
