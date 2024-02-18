using Arcana_Compiler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ErrorStatementNode : StatementNode {
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
    }
}
