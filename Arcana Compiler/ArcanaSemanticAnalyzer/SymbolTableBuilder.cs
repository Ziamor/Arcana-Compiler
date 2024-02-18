using Arcana_Compiler.Common;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class SymbolTableBuilder : IVisitor, ISymbolTableBuilder {
        private ISymbolTable? _symbolTable;

        public void BuildSymbolTable(ProgramNode rootNode, ISymbolTable symbolTable) {
            _symbolTable = symbolTable;
            Visit(rootNode);
        }

        public void Visit(ProgramNode node) {
            foreach (var import in node.Imports) {
                import.Accept(this);
            }

            foreach (var namespaceDeclaration in node.NamespaceDeclarations) {
                namespaceDeclaration.Accept(this);
            }
        }

        public void Visit(ImportDeclarationNode node) {
            Console.WriteLine("TODO, implement visit import node");
        }

        public void Visit(NamespaceDeclarationNode node) {
            if (_symbolTable == null)
                throw new InvalidOperationException("SymbolTable has not been built. Call BuildSymbolTable() first.");
            // Check if the namespace already exists
            var existingNamespaceSymbol = _symbolTable.FindSymbol(node.Name.ToString(), searchAllScopes: false) as NamespaceSymbol;

            NamespaceSymbol namespaceSymbol;
            if (existingNamespaceSymbol == null) {
                // If the namespace does not exist, create a new one and add it to the symbol table
                namespaceSymbol = new NamespaceSymbol(node.Name.ToString());
                _symbolTable.AddSymbol(namespaceSymbol);
            } else {
                // If the namespace already exists, reuse the existing symbol
                namespaceSymbol = existingNamespaceSymbol;
            }

            _symbolTable.EnterScope(namespaceSymbol); // Enter the new namespace scope

            foreach (var classDeclaration in node.ClassDeclarations) {
                classDeclaration.Accept(this);
            }

            foreach (var interfaceDeclaration in node.InerfaceDeclarations) {
                interfaceDeclaration.Accept(this);
            }

            _symbolTable.ExitScope();
        }

        public void Visit(ClassDeclarationNode node) {
            var classSymbol = new ClassSymbol(node.ClassName);

            if (_symbolTable == null)
                throw new InvalidOperationException("SymbolTable has not been built. Call BuildSymbolTable() first.");
            _symbolTable.AddSymbol(classSymbol);
            _symbolTable.EnterScope(classSymbol);

            foreach (var field in node.Fields) {
                field.Accept(this);
            }

            foreach (var method in node.Methods) {
                method.Accept(this);
            }

            foreach (var nestedClass in node.NestedClasses) {
                nestedClass.Accept(this);
            }

            _symbolTable.ExitScope();
        }

        public void Visit(FieldDeclarationNode node) {
            var fieldSymbol = new FieldSymbol(node.FieldName, node.FieldType.TypeName);

            if (_symbolTable == null)
                throw new InvalidOperationException("SymbolTable has not been built. Call BuildSymbolTable() first.");
            _symbolTable.AddSymbol(fieldSymbol);
        }

        public void Visit(InterfaceDeclarationNode node) { }
        public void Visit(MethodSignatureNode node) { }

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

            if (_symbolTable == null)
                throw new InvalidOperationException("SymbolTable has not been built. Call BuildSymbolTable() first.");
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

        public void Visit(ForLoopNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ForEachLoopNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ExpressionStatementNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ErrorStatementNode node) {
            throw new NotImplementedException();
        }
    }
}
