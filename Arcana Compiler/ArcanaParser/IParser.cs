﻿using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;

public interface IParser<T> where T : ASTNode {
    T Parse();
}