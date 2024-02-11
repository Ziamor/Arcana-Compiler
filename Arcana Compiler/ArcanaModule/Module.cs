namespace Arcana_Compiler.ArcanaModule {
    public class Module
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<string> SourceFiles { get; set; } = new List<string>();
        public List<Module> Dependencies { get; set; } = new List<Module>();

        public Module(string path)
        {
            Path = path;
            Name = new DirectoryInfo(path).Name;
        }

        public void AddSourceFile(string filePath)
        {
            SourceFiles.Add(filePath);
        }

        public void AddDependency(Module module)
        {
            if (!Dependencies.Contains(module))
            {
                Dependencies.Add(module);
            }
        }
    }

}
