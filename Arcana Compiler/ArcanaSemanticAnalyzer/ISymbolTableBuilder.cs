using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public interface ISymbolTableBuilder {
        void BuildSymbolTable(ProgramNode rootNode, ISymbolTable symbolTable);
    }
}
