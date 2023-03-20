﻿using Lib;

namespace TestCombineFIles
{
    // Tests within the same test class will not run in parallel against each other. 
    // So I do not need to bother about all files below being called File1.cs, File2.cs, etc.
    public class CoreTests : IDisposable
    {
        private readonly string tempDirectory;

        public CoreTests()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
        }

        public void Dispose()
        {
            Directory.Delete(tempDirectory, true);
        }

        [Fact]
        public void TestCombineSelectedFilesRecursive()
        {
            // Arrange
            string subDirectory = Path.Combine(tempDirectory, "SubDirectory");
            Directory.CreateDirectory(subDirectory);

            string file1 = Path.Combine(tempDirectory, "File1.cs");
            string file2 = Path.Combine(subDirectory, "File2.cs");

            File.WriteAllText(file1, "Console.WriteLine(\"Hello from File1\");");
            File.WriteAllText(file2, "Console.WriteLine(\"Hello from File2\");");

            var rootNode = new FileSystemNode(tempDirectory);
            rootNode.Children.Add(new FileSystemNode(file1, true));
            rootNode.Children.Add(new FileSystemNode(subDirectory));
            rootNode.Children[1].Children.Add(new FileSystemNode(file2, true));

            var fileSystem = new FileSystem();
            var core = new Core(fileSystem);

            string outputFilePath = Path.Combine(tempDirectory, "Combined.cs");

            // Act
            using (FileStream outputFileStream = File.Create(outputFilePath))
            {
                core.CombineSelectedFilesRecursive(rootNode, outputFileStream);
            }

            // Assert
            string expectedResult = $"// Combined from: {file1}{Environment.NewLine}" +
                                    "Console.WriteLine(\"Hello from File1\");" +
                                    $"{Environment.NewLine}" +
                                    $"// Combined from: {file2}{Environment.NewLine}" +
                                    "Console.WriteLine(\"Hello from File2\");" +
                                    $"{Environment.NewLine}";

            string actualResult = File.ReadAllText(outputFilePath);
            Assert.Equal(expectedResult, actualResult);
        }


        [Fact]
        public void TestContainsCsFile_WithCsFile()
        {
            // Arrange
            string subDirectory = Path.Combine(tempDirectory, "SubDirectory");
            Directory.CreateDirectory(subDirectory);

            string file1 = Path.Combine(subDirectory, "File1.cs");
            File.WriteAllText(file1, "Console.WriteLine(\"Hello from File1\");");

            var fileSystem = new FileSystem();
            var core = new Core(fileSystem);

            // Act
            bool result = core.ContainsCsFile(tempDirectory);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TestContainsCsFile_WithoutCsFile()
        {
            // Arrange
            string subDirectory = Path.Combine(tempDirectory, "SubDirectory");
            Directory.CreateDirectory(subDirectory);

            string file1 = Path.Combine(subDirectory, "File1.txt");
            File.WriteAllText(file1, "Hello from File1");

            var fileSystem = new FileSystem();
            var core = new Core(fileSystem);

            // Act
            bool result = core.ContainsCsFile(tempDirectory);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TestIsNotAutoGenerated_AutoGenerated()
        {
            // Arrange
            string file1 = Path.Combine(tempDirectory, "File1.cs");
            File.WriteAllText(file1, "// This file is auto-generated.\nConsole.WriteLine(\"Hello from File1\");");

            var fileSystem = new FileSystem();
            var core = new Core(fileSystem);

            // Act
            bool result = core.IsNotAutoGenerated(file1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TestIsNotAutoGenerated_NotAutoGenerated()
        {
            // Arrange
            string file1 = Path.Combine(tempDirectory, "File1.cs");
            File.WriteAllText(file1, "Console.WriteLine(\"Hello from File1\");");

            var fileSystem = new FileSystem();
            var core = new Core(fileSystem);

            // Act
            bool result = core.IsNotAutoGenerated(file1);

            // Assert
            Assert.True(result);
        }
    }
}