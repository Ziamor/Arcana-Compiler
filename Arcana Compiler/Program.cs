// See https://aka.ms/new-console-template for more information
using Arcana_Compiler.ArcanaModule;
using Arcana_Compiler.ArcanaModule.Arcana_Compiler.ArcanaModule;

Console.WriteLine("Starting compiler...");
//string filePath = "Arcana test scripts";
string filePath = "Arcana test scripts/StaticClassFieldMethod.arc";
//IModuleLoader loader = new FolderModuleLoader();
IModuleLoader loader = new SingleFileModuleLoader();

Module rootModule = loader.LoadModule(filePath);

Compiler compiler = new Compiler(rootModule);
compiler.Compile();