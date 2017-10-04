namespace LogData_CodeAnalyser15
{
    using Microsoft.CodeAnalysis;

    public class Reference
    {
        public Document RefDocument { get; private set; }
        public Location RefLocation { get; private set; }

        public string RefPositionInfo
        {
            get
            {
                return string.Format("{0}({1})", RefLocation.SourceTree.FilePath, RefLocation.SourceSpan.Start);
            }
        }

        public string CodeSnippet
        {
            get
            {
                if (RefLocation == null)
                {
                    return string.Empty;
                }

                var root = RefLocation.SourceTree.GetRoot();
                var node = root.FindNode(RefLocation.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true);

                SyntaxNode invocationNode = node.Parent;
                while (invocationNode != null)
                {
                    if (invocationNode.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.InvocationExpression))
                    {
                        break;
                    }
                    invocationNode = invocationNode.Parent;                  
                }

                return invocationNode != null 
                    ? invocationNode.GetText().ToString()
                    : string.Empty;
            }
        }

        public Reference(Document document, Location location)
        {
            //RefDocument = document;
            RefLocation = location;
        }
    }
}
