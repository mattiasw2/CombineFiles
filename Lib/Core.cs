using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CombineFiles;

namespace Lib {
    public static class Core {
        public static bool ContainsCsFile(string directory) {
            try {
                var csFiles = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);
                return csFiles.Length > 0;
            }
            catch (UnauthorizedAccessException) {
                return false;
            }
        }


        public static bool IsNotAutoGenerated(string filePath) {
            try {
                using (var reader = new StreamReader(filePath)) {
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


        public static void CombineSelectedFilesRecursive(FileSystemNode node, Stream output, IFileSystem fileSystem) {
            if (node.IsChecked) {
                string filePath = node.FullPath;
                if (!fileSystem.DirectoryExists(filePath)) {
                    byte[] commentBytes = Encoding.UTF8.GetBytes($"// Combined from: {filePath}{Environment.NewLine}");
                    output.Write(commentBytes, 0, commentBytes.Length);

                    byte[] buffer = new byte[4096];
                    using (Stream input = fileSystem.OpenRead(filePath)) {
                        int bytesRead;
                        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0) {
                            output.Write(buffer, 0, bytesRead);
                        }
                    }

                    byte[] newLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);
                    output.Write(newLineBytes, 0, newLineBytes.Length);
                }
            }

            foreach (FileSystemNode childNode in node.Children) {
                CombineSelectedFilesRecursive(childNode, output, fileSystem);
            }
        }
    }
}