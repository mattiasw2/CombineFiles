using System.Windows.Forms;
using Lib;

namespace CombineFiles {
    internal class Utils {
        public static FileSystemNode MapTreeNodeToFileSystemNode(TreeNode treeNode) {
            var fileSystemNode = new FileSystemNode(treeNode.FullPath, treeNode.Checked);

            foreach (TreeNode childTreeNode in treeNode.Nodes) {
                var childFileSystemNode = MapTreeNodeToFileSystemNode(childTreeNode);
                fileSystemNode.Children.Add(childFileSystemNode);
            }

            return fileSystemNode;
        }
    }
}