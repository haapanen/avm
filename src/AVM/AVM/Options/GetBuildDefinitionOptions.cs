using CommandLine;

namespace AVM.Options
{
    [Verb("get-build-definition")]
    public class GetBuildDefinitionOptions
    {
        [Option('d', "definition", HelpText = "Definition ID on Azure DevOps", Required = true)]
        public int DefinitionId { get; set; }
    }
}