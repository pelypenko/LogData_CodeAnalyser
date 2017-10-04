namespace LogData_CodeAnalyser15
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.FindSymbols;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Linq;

    public class CSharpSolution
    {
        public Solution Solution { get; private set; }

        public int AmountOfDocuments
        {
            get
            {
                var projs = new List<Project>(this.Solution.Projects);
                int result = projs.Sum(i => i.Documents.Count());
                return result;
            }
        }

        public CSharpSolution(string path)
        {
            this.LoadSolution(path);
        }

        private void LoadSolution(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            var workspace = Microsoft.CodeAnalysis.MSBuild.MSBuildWorkspace.Create();
            workspace.WorkspaceFailed += (o, e) => { Console.WriteLine(e.Diagnostic); };
            this.Solution = workspace.OpenSolutionAsync(filePath).Result;
        }

        public List<Reference> GetAllReferencesToMethod(string documentName, string methodName)
        {
            var document = FindDocument(documentName);
            if (document == null)
            {
                return null;
            }

            var treeRoot = document.GetSyntaxRootAsync().Result;
            var methodDeclaration = treeRoot
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .First(i => i.Identifier.ValueText.Equals(methodName, StringComparison.CurrentCultureIgnoreCase));

            var documentModel = document.GetSemanticModelAsync().Result;
            var method = documentModel.GetDeclaredSymbol(methodDeclaration);
            var methodReferences = SymbolFinder.FindReferencesAsync(method, this.Solution).Result;

            List<Reference> result = new List<Reference>();
            foreach (var mref in methodReferences)
            {
                result.AddRange(mref.Locations.Select(i => new Reference(i.Document, i.Location)).ToList());


                //result.AddRange(reference.Locations.Select(i => i.Document.FilePath).ToList());
            }

            return result;
        }

        private Document FindDocument(string documentName)
        {
            Document result = null;
            foreach (var project in Solution.Projects)
            {
                result = project.Documents.FirstOrDefault(i => i.Name.Equals(documentName, StringComparison.CurrentCultureIgnoreCase));
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }

        //public string GetCodeSnippet(Location location)
        //{
        //    if (location == null)
        //    {
        //        return string.Empty;
        //    }

        //    var root = location.SourceTree.GetRoot();
        //    var node = root.FindNode(location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true);
        //    return node != null
        //        ? node.GetText().ToString()
        //        : string.Empty;
        //}
    }
}
