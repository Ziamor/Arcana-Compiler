using Arcana_Compiler.ArcanaParser.Nodes;

namespace Arcana_Compiler.Common {
    public interface IVisitor {
        void Visit(ProgramNode node);
        void Visit(ImportDeclarationNode node);
        void Visit(NamespaceDeclarationNode node);
        void Visit(ClassDeclarationNode node);
        void Visit(ClassModifierNode node);
        void Visit(InterfaceDeclarationNode node);
        void Visit(FieldDeclarationNode node);
        void Visit(FieldModifierNode node);
        void Visit(TypeNode node);
        void Visit(MethodSignatureNode node);
        void Visit(LiteralNode node);
        void Visit(NullLiteralNode node);
        void Visit(ObjectInstantiationNode node);
        void Visit(ThisExpressionNode node);
        void Visit(ThisAssignmentNode node);
        void Visit(VariableAccessNode node);
        void Visit(VariableDeclarationNode node);
        void Visit(VariableAssignmentNode node);
        void Visit(UnaryOperationNode node);
        void Visit(BinaryOperationNode node);
        void Visit(MethodDeclarationNode node);
        void Visit(MethodModifierNode node);
        void Visit(MethodCallNode node);
        void Visit(ParameterNode node);
        void Visit(ChainedMethodCallNode node);
        void Visit(ChainedPropertyAccessNode node);
        void Visit(IfStatementNode node);
        void Visit(ReturnStatementNode node);
        void Visit(DestructuringAssignmentNode node);
        void Visit(ForLoopNode node);
        void Visit(ForEachLoopNode node);
        void Visit(ExpressionStatementNode node);
        void Visit(ErrorStatementNode node);
        void Visit(ArrayAccessNode node);
        void Visit(PrimaryExpressionNode node);
        void Visit(ArrayInitializationNode node);
    }
}
