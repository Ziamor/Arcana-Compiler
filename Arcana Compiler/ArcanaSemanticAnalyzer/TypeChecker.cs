using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;
using System;
using System.Linq;
using System.Collections.Generic;
using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class TypeChecker : IVisitor {
        private readonly SymbolTable _symbolTable;
        private readonly Dictionary<string, ImportDeclarationNode> _imports;

        public TypeChecker(SymbolTable symbolTable) {
            _symbolTable = symbolTable;
            _imports = new Dictionary<string, ImportDeclarationNode>();
        }

        public void Visit(ProgramNode node) {
            // Reset imports for this file
            _imports.Clear();

            // First, handle imports to know which namespaces are accessible
            foreach (var import in node.Imports) {
                import.Accept(this);
            }

            // Then, proceed to type checking within namespaces and their members
            foreach (var namespaceDeclaration in node.NamespaceDeclarations) {
                namespaceDeclaration.Accept(this);
            }
        }

        public void Visit(ImportDeclarationNode node) {
            // Assuming fully qualified namespace is the key
            var namespaceKey = node.ImportedNamespace.ToString();
            if (!_imports.ContainsKey(namespaceKey)) {
                _imports[namespaceKey] = node;
            }
        }

        public void Visit(NamespaceDeclarationNode node) {
            // Just to set context, not much to do unless you're checking for using directives that are not used.
            // Further validation happens in class, interface, and method validations.
            foreach (var classDeclaration in node.ClassDeclarations) {
                classDeclaration.Accept(this);
            }

            // Assuming similar pattern for interface declarations
            // foreach (var interfaceDeclaration in node.InterfaceDeclarations) {
            //     interfaceDeclaration.Accept(this);
            // }
        }

        public void Visit(ClassDeclarationNode node) {
            // Check fields and methods in class
            foreach (var field in node.Fields) {
                field.Accept(this);
            }

            foreach (var method in node.Methods) {
                method.Accept(this);
            }

            // If there are nested classes, they should also be visited here
            // Similar pattern as top-level class declarations
        }

        public void Visit(FieldDeclarationNode node) {
            // Ensure that the field's type is resolvable and valid
            ValidateType(node.FieldType.TypeName, node.FieldName);
        }

        public void Visit(MethodDeclarationNode node) {
            // Similar to FieldDeclarationNode, ensure method's return and parameter types are valid
            // Also, check method body if applicable, considering local variable declarations, assignments, etc.
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

        public void Visit(LiteralNode node) {
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
            throw new NotImplementedException();
        }

        public void Visit(VariableAccessNode node) {
            throw new NotImplementedException();
        }

        public void Visit(VariableDeclarationNode node) {
            throw new NotImplementedException();
        }

        public void Visit(VariableAssignmentNode node) {
            throw new NotImplementedException();
        }

        public void Visit(UnaryOperationNode node) {
            throw new NotImplementedException();
        }

        public void Visit(BinaryOperationNode node) {
            throw new NotImplementedException();
        }

        public void Visit(MethodModifierNode node) {
            throw new NotImplementedException();
        }

        public void Visit(MethodCallNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ParameterNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ChainedMethodCallNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ChainedPropertyAccessNode node) {
            throw new NotImplementedException();
        }

        public void Visit(IfStatementNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ReturnStatementNode node) {
            throw new NotImplementedException();
        }

        public void Visit(DestructuringAssignmentNode node) {
            throw new NotImplementedException();
        }

        private void ValidateType(string typeName, string identifier) {
            // This method should check if the type is defined in the symbol table or in the imports
            // It might be a complex type, so consider namespaces and possibly generics if your language supports them

            Symbol? foundSymbol = _symbolTable.FindSymbol(typeName, searchAllScopes: true);
            if (foundSymbol == null && !_imports.ContainsKey(typeName)) {
                throw new TypeCheckingException($"Type '{typeName}' for '{identifier}' is not defined or imported.");
            }
        }

        // Implement other Visit methods for different node types as needed, especially for expressions
    }

    public class TypeCheckingException : Exception {
        public TypeCheckingException(string message) : base(message) { }
    }
}
