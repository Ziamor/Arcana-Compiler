using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Arcana_Compiler.ArcanaModule;
using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser;
using Arcana_Compiler.ArcanaSemanticAnalyzer;
using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;
using Arcana_Compiler.Common;
using Arcana_Compiler.ArcanaModule.Arcana_Compiler.ArcanaModule;

public static class CompilerFactory {
    public static Compiler CreateCompiler(string configFilePath) {
        // Build configuration
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(configFilePath, optional: false, reloadOnChange: true);
        IConfigurationRoot configuration = builder.Build();

        // Read configuration values
        string? moduleLoaderType = configuration["ModuleLoader:Type"];
        string? filePath = configuration["ModuleLoader:FilePath"];
        string? folderPath = configuration["ModuleLoader:FolderPath"];

        // Determine the loader based on configuration
        IModuleLoader loader = moduleLoaderType switch {
            "SingleFile" => new SingleFileModuleLoader(),
            "Folder" => new FolderModuleLoader(),
            _ => throw new InvalidOperationException("Unknown ModuleLoader type specified in configuration.")
        };

        // Use the appropriate file path based on loader type
        string? pathToLoad = moduleLoaderType == "SingleFile" ? filePath : folderPath;

        if (string.IsNullOrEmpty(pathToLoad)) {
            throw new InvalidOperationException("The path to load the module cannot be null or empty.");
        }

        IModule rootModule = loader.LoadModule(pathToLoad);

        IParser parser = new Parser();
        ILexer lexer = new Lexer();

        ISymbolTable symbolTable = new SymbolTable();
        ISymbolTableBuilder symbolTableBuilder = new SymbolTableBuilder();

        // Create and return the compiler instance
        return new Compiler(rootModule, parser, lexer, symbolTable, symbolTableBuilder);
    }
}
