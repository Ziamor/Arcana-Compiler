// See https://aka.ms/new-console-template for more information
using Arcana_Compiler;

Console.WriteLine("Starting compiler...");
string filePath = "test.arc";
Compiler compiler = new Compiler(filePath);
compiler.Compile();