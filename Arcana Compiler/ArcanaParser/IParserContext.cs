using Arcana_Compiler.ArcanaParser.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaParser {
    public interface IParserBaseContext {
    }
    public interface IParserContext<TNode> : IParserBaseContext where TNode : ASTNode {
    }
}
