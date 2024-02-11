// See https://aka.ms/new-console-template for more information
using Arcana_Compiler.ArcanaModule;

Console.WriteLine("Starting compiler...");
string filePath = "Arcana test scripts";

IModuleLoader loader = new FolderModuleLoader();
Module rootModule = loader.LoadModule(filePath);

Compiler compiler = new Compiler(rootModule);
compiler.Compile();