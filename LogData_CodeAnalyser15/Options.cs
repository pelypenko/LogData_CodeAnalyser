namespace LogData_CodeAnalyser15
{
    using CommandLine;
    using CommandLine.Text;

    class Options
    {
        [Option('p', "SolutionPath", Required = true, HelpText = "Path to Solution")]
        public string SolutionPath { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}