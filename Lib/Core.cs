using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lib {
    public class Core {
        private readonly IFileSystem fileSystem;

        public Core(IFileSystem fileSystem0) {
            fileSystem = fileSystem0;
        }

        public bool ContainsCsFile(string directory) {
            try {
                var csFiles = fileSystem.DirectoryGetFiles(directory, "*.cs", SearchOption.AllDirectories);
                return csFiles.Length > 0;
            }
            catch (UnauthorizedAccessException) {
                return false;
            }
        }


        public bool IsNotAutoGenerated(string filePath) {
            try {
                using (StreamReader reader = fileSystem.OpenStreamReader(filePath)) {
                    var autogeneratedPattern = new Regex(@"\b(?:auto generated|auto-generated|autogenerated)\b", RegexOptions.IgnoreCase);

                    for (int i = 0; i < 5; i++) {
                        if (reader.EndOfStream) {
                            break;
                        }

                        string line = reader.ReadLine();
                        if (line != null && autogeneratedPattern.IsMatch(line)) {
                            return false;
                        }
                    }

                    return true;
                }
            }
            catch (IOException) {
                return false;
            }
        }


        public void CombineSelectedFilesRecursive(FileSystemNode node, Stream output) {
            if (node.IsChecked) {
                string filePath = node.FullPath;
                if (!fileSystem.DirectoryExists(filePath)) {
                    byte[] commentBytes = Encoding.UTF8.GetBytes($"// Combined from: {filePath}{Environment.NewLine}");
                    output.Write(commentBytes, 0, commentBytes.Length);

                    string content;
                    using (StreamReader reader = new StreamReader(fileSystem.OpenRead(filePath), Encoding.UTF8)) {
                        content = reader.ReadToEnd();
                    }

                    content = ProcessContent(content);
                    byte[] contentBytes = Encoding.UTF8.GetBytes(content);
                    output.Write(contentBytes, 0, contentBytes.Length);

                    byte[] newLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);
                    output.Write(newLineBytes, 0, newLineBytes.Length);
                }
            }

            foreach (FileSystemNode childNode in node.Children) {
                CombineSelectedFilesRecursive(childNode, output);
            }
        }

        private string ProcessContent(string content) {
            content = RemoveComments(content);
            content = KeepMembers(content);
            content = RemoveEmptyLines(content);
            return content;
        }

        private string KeepMembers(string content) {
            var tree = CSharpSyntaxTree.ParseText(content);
            var root = tree.GetCompilationUnitRoot();
            var compilation = CSharpCompilation.Create(null).AddSyntaxTrees(tree).AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            var semanticModel = compilation.GetSemanticModel(tree);

            var fields = root.DescendantNodes().OfType<FieldDeclarationSyntax>();
            var properties = root.DescendantNodes().OfType<PropertyDeclarationSyntax>();
            var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

            var sb = new StringBuilder();

            foreach (var field in fields) {
                var fieldType = field.Declaration.Type.ToString();
                var fieldName = field.Declaration.Variables.First().Identifier.ToString();
                sb.AppendLine($"{fieldType} {fieldName}");
            }

            foreach (var property in properties) {
                var propertyType = property.Type.ToString();
                var propertyName = property.Identifier.ToString();
                sb.AppendLine($"{propertyType} {propertyName}");
            }

            var rewriter = new MethodBodyRemover(semanticModel);
            foreach (var methodDeclaration in methodDeclarations) {
                var newMethodDeclaration = rewriter.Visit(methodDeclaration);
                sb.AppendLine(newMethodDeclaration.ToFullString());
            }

            return sb.ToString();
        }


        private string RemoveComments(string content) {
            string linePattern = @"//.*?$";
            string blockPattern = @"/\*[\s\S]*?\*/";
            string pattern = $@"({linePattern}|{blockPattern})";
            return Regex.Replace(content, pattern, "", RegexOptions.Multiline);
        }


        private class MethodBodyRemover : CSharpSyntaxRewriter {
            // Set this flag to true if you want to keep method call arguments, otherwise set it to false
            private readonly bool _keepArguments = false;

            private readonly SemanticModel _semanticModel;

            public MethodBodyRemover(SemanticModel semanticModel) {
                _semanticModel = semanticModel;
            }

            public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) {
                var bodyWithComments = ExtractMethodCallsAsComments(node.Body);
                return node.WithBody(bodyWithComments);
            }

            public override SyntaxNode VisitLocalFunctionStatement(LocalFunctionStatementSyntax node) {
                var bodyWithComments = ExtractMethodCallsAsComments(node.Body);
                return node.WithBody(bodyWithComments);
            }

            private BlockSyntax ExtractMethodCallsAsComments(BlockSyntax body) {
                if (body == null) {
                    return SyntaxFactory.Block();
                }

                var methodCalls = body.DescendantNodes().OfType<InvocationExpressionSyntax>().ToList();
                var uniqueMethodCalls = new HashSet<InvocationExpressionSyntax>(methodCalls, new ExpressionSyntaxComparer());
                var comments = uniqueMethodCalls.Select(call => CreateCommentFromMethodCall(call)).ToList();

                var statementsWithLeadingTrivia = comments.Select(comment =>
                    SyntaxFactory.EmptyStatement().WithLeadingTrivia(comment)).ToArray();

                return SyntaxFactory.Block(statementsWithLeadingTrivia);
            }

            private SyntaxTrivia CreateCommentFromMethodCall(InvocationExpressionSyntax call) {
                var ignoredTypes = new HashSet<string> { "ILogger", "String", "Log" };
                var ignoredNamespaces = new HashSet<string> { "System.Diagnostics", "Microsoft.Extensions.Logging", "System" };
                bool methodNameOnly = false;

                string commentText;
                var typeInfo = _semanticModel.GetTypeInfo(call.Expression);
                var typeName = typeInfo.Type?.Name;
                var namespaceName = typeInfo.Type?.ContainingNamespace?.ToString();
                var methodName = methodNameOnly
                    ? call.Expression.ToString().Split('.').Last().Split('(').First()
                    : call.Expression.ToString();
                ;

                if (typeName != null && ignoredTypes.Contains(typeName)) {
                    return SyntaxFactory.Comment("");
                }

                if (namespaceName != null && ignoredNamespaces.Contains(namespaceName)) {
                    return SyntaxFactory.Comment("");
                }

                if (ignoredTypes.Contains(methodName)) {
                    return SyntaxFactory.Comment("");
                }

                if (methodName.ToLower().Contains("log")) {
                    return SyntaxFactory.Comment("");
                }

                if (_keepArguments) {
                    commentText = $"\n/* {call} */";
                }
                else {
                    commentText = $"\n// {methodName}(..)";
                }

                return SyntaxFactory.Comment(commentText);
            }
        }


        public static string RemoveEmptyLines(string content) {
            content = content.Replace("\r\n", "\n");
            content = content.Replace("\r", "");
            var lines = content.Split(new[] { '\n' }, StringSplitOptions.None);
            var nonEmptyLines = lines.Select(line => line.Trim()).Where(line => !string.IsNullOrEmpty(line)).ToArray();
            return string.Join("\n", nonEmptyLines);
        }

        private class ExpressionSyntaxComparer : IEqualityComparer<InvocationExpressionSyntax> {
            public bool Equals(InvocationExpressionSyntax x, InvocationExpressionSyntax y) {
                if (x == null || y == null) return false;
                return x.ToString() == y.ToString();
            }

            public int GetHashCode(InvocationExpressionSyntax obj) {
                if (obj == null) return 0;
                return obj.ToString().GetHashCode();
            }
        }
    }
}