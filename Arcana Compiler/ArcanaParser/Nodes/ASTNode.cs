using Arcana_Compiler.Common;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ArcanaCompilerTests")]

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public abstract class ASTNode {
        public abstract void Accept(IVisitor visitor);
    }
}
