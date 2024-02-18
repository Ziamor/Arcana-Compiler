namespace Arcana_Compiler.ArcanaModule {

    public class Module : IModule {
        public string Name { get; set; }
        public string Path { get; set; }
        private List<string> _sourceFiles = new List<string>();
        public IEnumerable<string> SourceFiles => _sourceFiles.AsReadOnly();
        public List<IModule> _dependencies { get; set; } = new List<IModule>();
        public IEnumerable<IModule> Dependencies => _dependencies.AsReadOnly();

        public Module(string path) {
            Path = path;
            Name = new DirectoryInfo(path).Name;
        }

        public void AddSourceFile(string filePath) {
            _sourceFiles.Add(filePath);
        }

        public void AddDependency(IModule module) {
            if (!_dependencies.Contains(module)) {
                _dependencies.Add(module);
            }
        }
    }
}
