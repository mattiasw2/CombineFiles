using System.Collections.Generic;
using System.IO;

namespace CombineFiles {
    public class FileSystemNode
    {
        public string FullPath { get; set; }
        public bool IsChecked { get; set; }
        public List<FileSystemNode> Children { get; set; } = new List<FileSystemNode>();

        public FileSystemNode(string fullPath, bool isChecked = false)
        {
            FullPath = fullPath;
            IsChecked = isChecked;
        }
    }

    public interface IFileSystem
    {
        bool DirectoryExists(string path);
        Stream OpenRead(string path);
    }

    public class FileSystem : IFileSystem
    {
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }
    }

}