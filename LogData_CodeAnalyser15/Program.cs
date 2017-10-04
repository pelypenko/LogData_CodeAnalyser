namespace LogData_CodeAnalyser15
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                Console.WriteLine("Cannot parse arguments");
                return;
            }

            AnalyseUsageLog4NetLoggers(options.SolutionPath);
        }

        static void AnalyseUsageLog4NetLoggers(string solutionPath)
        {
            Console.WriteLine("Starting loading solution");
            var solution = new CSharpSolution(solutionPath);
            if (solution == null)
            {
                Console.WriteLine("Cannot load solution");
                return;
            }
            Console.WriteLine(string.Format("Loaded {0} documents", solution.AmountOfDocuments));

            var documentName = @"Log4NetExtensions.cs";
            var methodName = @"LogAndPublishErrorFormat";

            Console.WriteLine(string.Format("Starting searching for {0}->{1}", documentName, methodName));
            var references = solution.GetAllReferencesToMethod(documentName, methodName);

            Console.WriteLine(string.Format("Found {0} references", references.Count));
            references.ForEach(i =>
                {
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine(i.RefPositionInfo);
                    Console.WriteLine(i.CodeSnippet);
                });


            Console.WriteLine();
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
        }
    }
}
