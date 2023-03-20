using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;

namespace Lib {
    public class FileSystemNode {
        public string FullPath { get; set; }
        public bool IsChecked { get; set; }
        public List<FileSystemNode> Children { get; set; } = new List<FileSystemNode>();

        public FileSystemNode(string fullPath, bool isChecked = false) {
            FullPath = fullPath;
            IsChecked = isChecked;
        }
    }

    public interface IFileSystem {
        bool DirectoryExists(string path);
        Stream OpenRead(string path);
        StreamReader OpenStreamReader(string path);
        string[] DirectoryGetFiles(string path, string searchPattern, SearchOption searchOption);
    }

    public class FileSystem : IFileSystem {
        public bool DirectoryExists(string path) {
            return Directory.Exists(path);
        }

        public Stream OpenRead(string path) {
            return File.OpenRead(path);
        }

        public StreamReader OpenStreamReader(string path)
        {
            return new StreamReader(path);
        }

        public string[] DirectoryGetFiles(string path, string searchPattern, SearchOption searchOption) {
            return Directory.GetFiles(path, searchPattern, searchOption);
        }
    }
}