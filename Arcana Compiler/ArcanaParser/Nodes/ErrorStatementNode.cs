using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ErrorStatementNode : StatementNode {
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
    }
}
