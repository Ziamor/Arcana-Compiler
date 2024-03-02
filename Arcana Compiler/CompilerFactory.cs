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
        IConfigurationRoot configuration = BuildConfiguration(configFilePath);
        IModule rootModule = LoadModule(configuration);
        return BuildCompiler(rootModule);
    }

    private static IConfigurationRoot BuildConfiguration(string configFilePath) {
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(configFilePath, optional: false, reloadOnChange: true);
        return builder.Build();
    }

    private static IModule LoadModule(IConfigurationRoot configuration) {
        // Read configuration values
        string moduleLoaderType = configuration["ModuleLoader:Type"] ?? throw new InvalidOperationException("ModuleLoader type must be specified.");
        string pathToLoad = GetPathToLoad(configuration, moduleLoaderType);

        // Determine and use the loader based on configuration
        IModuleLoader loader = GetModuleLoader(moduleLoaderType);
        return loader.LoadModule(pathToLoad);
    }

    private static string GetPathToLoad(IConfigurationRoot configuration, string moduleLoaderType) {
        string? filePath = configuration["ModuleLoader:FilePath"];
        string? folderPath = configuration["ModuleLoader:FolderPath"];

        string? pathToLoad = moduleLoaderType == "SingleFile" ? filePath : folderPath;

        if (string.IsNullOrEmpty(pathToLoad)) {
            throw new InvalidOperationException("The path to load the module cannot be null or empty.");
        }

        return pathToLoad;
    }

    private static IModuleLoader GetModuleLoader(string moduleLoaderType) {
        return moduleLoaderType switch {
            "SingleFile" => new SingleFileModuleLoader(),
            "Folder" => new FolderModuleLoader(),
            _ => throw new InvalidOperationException($"Unknown ModuleLoader type: {moduleLoaderType}")
        };
    }

    private static Compiler BuildCompiler(IModule rootModule) {
        ILexer lexer = new Lexer();
        ISymbolTable symbolTable = new SymbolTable();
        ISymbolTableBuilder symbolTableBuilder = new SymbolTableBuilder();

        return new Compiler(rootModule, lexer, symbolTable, symbolTableBuilder);
    }
}
