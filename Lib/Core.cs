using System;
using System.IO;
using System.Text;
using CombineFiles;

static public class Core {



    public static void CombineSelectedFilesRecursive(FileSystemNode node, Stream output, IFileSystem fileSystem)
    {
        if (node.IsChecked)
        {
            string filePath = node.FullPath;
            if (!fileSystem.DirectoryExists(filePath))
            {
                byte[] commentBytes = Encoding.UTF8.GetBytes($"// Combined from: {filePath}{Environment.NewLine}");
                output.Write(commentBytes, 0, commentBytes.Length);

                byte[] buffer = new byte[4096];
                using (Stream input = fileSystem.OpenRead(filePath))
                {
                    int bytesRead;
                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, bytesRead);
                    }
                }

                byte[] newLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);
                output.Write(newLineBytes, 0, newLineBytes.Length);
            }
        }

        foreach (FileSystemNode childNode in node.Children)
        {
            CombineSelectedFilesRecursive(childNode, output, fileSystem);
        }
    }
}