using Arcana_Compiler.ArcanaParser.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaParser.Parsers.Contexts {
    public class BinaryOperationParserContext : IParserContext<BinaryOperationNode> {
        public int Precedence { get; }
        public ExpressionNode Left { get; }

        public BinaryOperationParserContext(int precedence, ExpressionNode left) {
            Precedence = precedence;
            Left = left;
        }
    }
}