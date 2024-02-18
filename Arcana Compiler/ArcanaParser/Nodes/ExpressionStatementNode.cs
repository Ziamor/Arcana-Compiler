using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    /// <summary>
    /// Represents an expression used as a statement in the source code.
    /// </summary>
    /// <remarks>
    /// This node type is used to wrap expressions that are intended to be executed
    /// for their side effects rather than their values, enabling expressions to be used
    /// in statement contexts. As an example, this is used for method calls that return 
    /// no value.
    /// </remarks>
    public class ExpressionStatementNode : StatementNode {
        public ExpressionNode Expression { get; }

        public ExpressionStatementNode(ExpressionNode expression) {
            Expression = expression;
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
    }
}
