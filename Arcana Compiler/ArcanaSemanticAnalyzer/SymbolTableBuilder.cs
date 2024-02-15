using Arcana_Compiler.Common;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.ArcanaSemanticAnalyzer;
using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class SymbolTableBuilder : IVisitor {
        private readonly SymbolTable _symbolTable;

        public SymbolTableBuilder(SymbolTable symbolTable) {
            _symbolTable = symbolTable;
        }

        public void Visit(ProgramNode node) {
            // The ProgramNode doesn't directly correspond to a symbol but serves as the root for the AST.
            foreach (var import in node.Imports) {
                import.Accept(this);
            }

            foreach (var namespaceDeclaration in node.NamespaceDeclarations) {
                namespaceDeclaration.Accept(this);
            }
        }

        public void Visit(ImportDeclarationNode node) {
            // Handle imports if necessary for symbol resolution, but not directly related to symbol table population.
        }

        public void Visit(NamespaceDeclarationNode node) {
            // Assuming NamespaceSymbol is a type of Symbol for namespaces.
            var namespaceSymbol = new NamespaceSymbol(node.Name.ToString());
            _symbolTable.AddSymbol(namespaceSymbol); // Add the namespace symbol to the current scope
            _symbolTable.EnterScope(namespaceSymbol); // Enter the new namespace scope

            foreach (var classDeclaration in node.ClassDeclarations) {
                classDeclaration.Accept(this);
            }

            // If there are interface declarations, they would be processed here as well.

            _symbolTable.ExitScope(); // Exit the namespace scope once all children are processed
        }

        public void Visit(ClassDeclarationNode node) {
            // Assuming ClassSymbol is a type of Symbol for classes.
            var classSymbol = new ClassSymbol(node.ClassName);
            _symbolTable.AddSymbol(classSymbol); // Add the class symbol within the current scope (namespace or outer class)
            _symbolTable.EnterScope(classSymbol);

            foreach (var field in node.Fields) {
                field.Accept(this);
            }

            foreach(var method in node.Methods) { 
                method.Accept(this); 
            }

            // TODO NESTED CLASSES

            _symbolTable.ExitScope(); // Exit the class scope once all members are processed
        }

        public void Visit(FieldDeclarationNode node) {
            // Assuming FieldSymbol is a type of Symbol for fields.
            var fieldSymbol = new FieldSymbol(node.FieldName, node.FieldType.TypeName);
            _symbolTable.AddSymbol(fieldSymbol); // Add the field symbol within the current class scope
        }

        // Implementations for other AST node visits would follow a similar pattern.

        // Placeholder methods for the interface, to be implemented as needed.
        public void Visit(InterfaceDeclarationNode node) { /* Implementation here */ }
        public void Visit(MethodSignatureNode node) { /* Implementation here */ }

        public void Visit(ClassModifierNode node) {
            throw new NotImplementedException();
        }

        public void Visit(FieldModifierNode node) {
            throw new NotImplementedException();
        }

        public void Visit(TypeNode node) {
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

        public void Visit(MethodDeclarationNode node) {
            var parameterTypes = node.Parameters.Select(p => p.ParameterType.TypeName).ToList();
            var returnTypes = node.ReturnTypes.Select(rt => rt.TypeName).ToList();
            var signature = new MethodSignature(node.MethodName, parameterTypes, returnTypes);

            var methodSymbol = new MethodSymbol(signature);

            _symbolTable.AddSymbol(methodSymbol);
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
        // Add other visit methods as per the IVisitor interface definition.
    }
}
