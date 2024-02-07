using Arcana_Compiler.ArcanaParser.Nodes;

namespace Arcana_Compiler.Common {
    public interface IVisitor {
        void Visit(ProgramNode node);
        void Visit(ImportDeclarationNode node);
        void Visit(ClassDeclarationNode node);
        void Visit(FieldDeclarationNode node);
        void Visit(TypeNode node);
        void Visit(LiteralNode node);
        void Visit(NullLiteralNode node);
        void Visit(ObjectInstantiationNode node);
        void Visit(VariableAccessNode node);
        void Visit(VariableDeclarationNode node);
        void Visit(VariableAssignmentNode node);
        void Visit(UnaryOperationNode node);
        void Visit(BinaryOperationNode node);
        void Visit(MethodDeclarationNode node);
        void Visit(MethodCallNode node);
        void Visit(ParameterNode node);
        void Visit(ChainedMethodCallNode node);
        void Visit(ChainedPropertyAccessNode node);
        void Visit(IfStatementNode node);
    }
}
