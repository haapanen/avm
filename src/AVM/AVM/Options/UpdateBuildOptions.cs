using CommandLine;

namespace AVM.Options
{
    [Verb("update")]
    public class UpdateBuildOptions
    {
        [Option('d', "definition", HelpText = "Azure DevOps build definition ID", Required = true)]
        public int DefinitionId { get; set; }
        [Option('s', "source", HelpText = "Source file", Required = true)]
        public string SourceFile { get; set; }
    }
}