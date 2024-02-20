﻿using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;

public interface IParserOld {
    ProgramNode Parse(ILexer lexer, out ErrorReporter reporter);
}