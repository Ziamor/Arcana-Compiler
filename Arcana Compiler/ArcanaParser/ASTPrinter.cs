using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;
using System.Text;

public class ASTPrinter : IVisitor {
    private StringBuilder result = new StringBuilder();
    private string indent = "";

    public string Print(ASTNode node) {
        node.Accept(this);
        return result.ToString();
    }

    private void IncreaseIndent() => indent += "  ";
    private void DecreaseIndent() => indent = indent.Substring(0, indent.Length - 2);

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

    public void Visit(ProgramNode node) {
        result.AppendLine(indent + "Program");
        IncreaseIndent();
        foreach (var import in node.Imports) {
            import.Accept(this);
        }
        foreach (var namespaceDeclarations in node.NamespaceDeclarations) {
            Visit(namespaceDeclarations);
        }
        DecreaseIndent();
    }

    public void Visit(NamespaceDeclarationNode node) {
        result.AppendLine($"{indent}Namespace: {node.Name}");
        IncreaseIndent();
        foreach (var classDecl in node.ClassDeclarations) {
            classDecl.Accept(this);
        }
        DecreaseIndent();
    }

    public void Visit(ImportDeclarationNode node) {
        result.AppendLine($"{indent}Import: {node.ImportedNamespace}");
    }
    public void Visit(ClassDeclarationNode node) {
        result.AppendLine($"{indent}Class: {node.ClassName}");
        IncreaseIndent();
        foreach (var field in node.Fields) {
            field.Accept(this);
        }
        foreach (var method in node.Methods) {
            method.Accept(this);
        }
        DecreaseIndent();
    }

    public void Visit(FieldDeclarationNode node) {
        result.AppendLine($"{indent}Field Decl: {node.FieldName} ({node.FieldType})");
    }
    public void Visit(LiteralNode node) {
        result.AppendLine($"{indent}Literal: {node.Value}");
    }

    public void Visit(MethodDeclarationNode node) {
        result.AppendLine($"{indent}Method: {node.MethodName}");
        IncreaseIndent();
        foreach (var param in node.Parameters) {
            param.Accept(this);
        }
        foreach (var statement in node.Body) {
            statement.Accept(this); // Assuming statements are ASTNode and Accept method is implemented
        }
        if (node.Body.Count == 0) {
            result.AppendLine(indent + "Empty");
        }
        DecreaseIndent();
    }

    public void Visit(ParameterNode node) {
        result.AppendLine($"{indent}Parameter: {node.ParameterName} ({node.ParameterType})");
    }

    public void Visit(MethodCallNode node) {
        result.AppendLine($"{indent}Method Call: {node.MethodName}");
        IncreaseIndent();
        foreach (var argument in node.Arguments) {
            argument.Accept(this);
        }
        DecreaseIndent();
    }

    public void Visit(VariableDeclarationNode node) {
        string initialValueOutput = node.InitialValue != null ? $" = {PrintExpression(node.InitialValue)}" : "";
        result.AppendLine($"{indent}Variable Decl: {node.Name} ({node.Type}){initialValueOutput}");
    }

    public void Visit(VariableAssignmentNode node) {
        result.AppendLine($"{indent}Variable: {node.VariableName} = {PrintExpression(node.AssignedExpression)}");
    }

    public void Visit(IfStatementNode node) {
        foreach (var (Condition, Statements) in node.ConditionsAndStatements) {
            result.AppendLine(indent + "If Condition:");
            IncreaseIndent();
            Condition.Accept(this);
            foreach (var statement in Statements) {
                statement.Accept(this);
            }
            DecreaseIndent();
        }
        if (node.ElseStatements != null) {
            result.AppendLine(indent + "Else:");
            IncreaseIndent();
            foreach (var elseStatement in node.ElseStatements) {
                elseStatement.Accept(this);
            }
            DecreaseIndent();
        }
    }

    public void Visit(ClassModifierNode node) {
        throw new NotImplementedException();
    }

    public void Visit(InterfaceDeclarationNode node) {
        throw new NotImplementedException();
    }

    public void Visit(FieldModifierNode node) {
        throw new NotImplementedException();
    }

    public void Visit(TypeNode node) {
        throw new NotImplementedException();
    }

    public void Visit(MethodSignatureNode node) {
        throw new NotImplementedException();
    }

    public void Visit(NullLiteralNode node) {
        throw new NotImplementedException();
    }

    public void Visit(ObjectInstantiationNode node) {
        throw new NotImplementedException();
    }

    public void Visit(ThisExpressionNode node) {
        throw new NotImplementedException();
    }

    public void Visit(ThisAssignmentNode node) {
        // Assuming ThisAssignmentNode has properties like VariableName and AssignedExpression
        result.AppendLine($"{indent}This Assignment: {node.IdentifierName} = {PrintExpression(node.ValueExpression)}");
    }

    public void Visit(VariableAccessNode node) {
        result.AppendLine($"{indent}Variable Access: {node.QualifiedName}");
    }

    public void Visit(UnaryOperationNode node) {
        string operand = PrintExpression(node.Operand);
        result.AppendLine($"{indent}Unary Operation: {node.Operator}{operand}");
    }

    public void Visit(BinaryOperationNode node) {
        string left = PrintExpression(node.Left);
        string right = PrintExpression(node.Right);
        result.AppendLine($"{indent}Binary Operation: {left} {node.Operator} {right}");
    }


    public void Visit(MethodModifierNode node) {
        throw new NotImplementedException();
    }

    public void Visit(ChainedMethodCallNode node) {
        throw new NotImplementedException();
    }

    public void Visit(ChainedPropertyAccessNode node) {
        throw new NotImplementedException();
    }

    public void Visit(ReturnStatementNode node) {
        result.AppendLine($"{indent}{node}");
    }


    public void Visit(DestructuringAssignmentNode node) {
        result.AppendLine($"{indent}{node}");
    }

    public void Visit(ForLoopNode node) {
        result.AppendLine($"{indent}For Loop:");
        IncreaseIndent();

        // Print initialization
        result.AppendLine($"{indent}Initialization: {PrintExpression(node.Initialization)}");

        // Print condition
        result.AppendLine($"{indent}Condition: {PrintExpression(node.Condition)}");

        // Print increment
        result.AppendLine($"{indent}Increment: {PrintExpression(node.Increment)}");

        // Print body
        result.AppendLine($"{indent}Body:");
        IncreaseIndent();
        foreach (var statement in node.Body) {
            statement.Accept(this);
        }
        if (node.Body.Count == 0) {
            result.AppendLine(indent + "Empty");
        }
        DecreaseIndent();

        DecreaseIndent();
    }

    public void Visit(ForEachLoopNode node) {
        result.AppendLine($"{indent}ForEach Loop:");
        IncreaseIndent();

        // Print variable
        result.AppendLine($"{indent}Variable: {node.Variable.Name} ({node.Variable.Type})");

        // Print collection
        result.AppendLine($"{indent}Collection: {PrintExpression(node.Collection)}");

        // Print body
        result.AppendLine($"{indent}Body:");
        IncreaseIndent();
        foreach (var statement in node.Body) {
            statement.Accept(this);
        }
        if (node.Body.Count == 0) {
            result.AppendLine(indent + "Empty");
        }
        DecreaseIndent();

        DecreaseIndent();
    }

    public void Visit(ExpressionStatementNode node) {
        node.Expression.Accept(this);
    }

    public void Visit(ErrorStatementNode node) {
        result.AppendLine($"{indent}Statement parse error");
    }
}
