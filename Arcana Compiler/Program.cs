using Microsoft.Extensions.Configuration;
using Arcana_Compiler.ArcanaModule;
using Arcana_Compiler.ArcanaModule.Arcana_Compiler.ArcanaModule;

// Build configuration
IConfigurationBuilder builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Folder.json", optional: false, reloadOnChange: true);
IConfigurationRoot configuration = builder.Build();

// Read configuration
string? moduleLoaderType = configuration["ModuleLoader:Type"];
string? filePath = configuration["ModuleLoader:FilePath"];
string? folderPath = configuration["ModuleLoader:FolderPath"];

Console.WriteLine("Starting compiler...");

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

Module rootModule = loader.LoadModule(pathToLoad);

Compiler compiler = new Compiler(rootModule);
compiler.Compile();