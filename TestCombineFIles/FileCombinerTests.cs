﻿using System.Text;
using System.Text.RegularExpressions;
using Lib;
using Moq;

namespace TestCombineFIles;

public class FileCombinerTests {
    public static int CountIncludedFiles(string combinedFileContent) {
        var filePattern = new Regex(@"^// Combined from: .*$", RegexOptions.Multiline);
        return filePattern.Matches(combinedFileContent).Count;
    }

    [Fact]
    public void CombineSelectedFilesRecursive_CombinesCorrectNumberOfFiles() {
        // Arrange
        var mockFileSystem = new Mock<IFileSystem>();
        Core core = new Core(mockFileSystem.Object);

        string file1Content = "File 1 content";
        string file2Content = "File 2 content";
        string file3Content = "File 3 content";

        using (var file1Stream = new MemoryStream(Encoding.UTF8.GetBytes(file1Content)))
        using (var file2Stream = new MemoryStream(Encoding.UTF8.GetBytes(file2Content)))
        using (var file3Stream = new MemoryStream(Encoding.UTF8.GetBytes(file3Content))) {
            mockFileSystem.Setup(fs => fs.DirectoryExists(It.IsAny<string>())).Returns(false);
            mockFileSystem.Setup(fs => fs.OpenRead("file1.cs")).Returns(file1Stream);
            mockFileSystem.Setup(fs => fs.OpenRead("file2.cs")).Returns(file2Stream);
            mockFileSystem.Setup(fs => fs.OpenRead("file3.cs")).Returns(file3Stream);

            var rootNode = new FileSystemNode("root");
            rootNode.Children.Add(new FileSystemNode("file1.cs", true));
            rootNode.Children.Add(new FileSystemNode("file2.cs"));
            rootNode.Children.Add(new FileSystemNode("file3.cs", true));

            using (var outputStream = new MemoryStream()) {
                // Act
                core.CombineSelectedFilesRecursive(rootNode, outputStream);

                // Assert
                outputStream.Seek(0, SeekOrigin.Begin);
                string combinedFileContent = Encoding.UTF8.GetString(outputStream.ToArray());
                int includedFiles = CountIncludedFiles(combinedFileContent);
                Assert.Equal(2, includedFiles);
            }
        }
    }
}