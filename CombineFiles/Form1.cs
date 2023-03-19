﻿using CombineFiles;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FileCombiner
{
    public partial class Form1 : Form
    {
        private string lastOutputFile = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            treeViewFiles.Nodes.Clear();
            string initialDirectory = "C:\\data3\\ssc\\sss7"; Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            LoadDirectory(initialDirectory);
        }

        private void LoadDirectory(string directory)
        {
            TreeNode rootNode = new TreeNode(directory);
            treeViewFiles.Nodes.Add(rootNode);
            LoadSubDirectories(directory, rootNode);
        }

        private void LoadSubDirectories(string directory, TreeNode node)
        {
            try
            {
                var subDirs = Directory.GetDirectories(directory);
                foreach (var dir in subDirs)
                {
                    if (ContainsCsFile(dir))
                    {
                        TreeNode dirNode = new TreeNode(Path.GetFileName(dir));
                        node.Nodes.Add(dirNode);
                        LoadSubDirectories(dir, dirNode);
                    }
                }

                var files = Directory.GetFiles(directory, "*.cs");
                foreach (var file in files)
                {
                    if (IsNotAutoGenerated(file))
                    {
                        TreeNode fileNode = new TreeNode(Path.GetFileName(file));
                        node.Nodes.Add(fileNode);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Skip directories that we don't have permission to access
            }
        }

        private bool IsNotAutoGenerated(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    var autogeneratedPattern = new Regex(@"\b(?:auto generated|auto-generated|autogenerated)\b", RegexOptions.IgnoreCase);

                    for (int i = 0; i < 5; i++)
                    {
                        if (reader.EndOfStream)
                        {
                            break;
                        }

                        string line = reader.ReadLine();
                        if (line != null && autogeneratedPattern.IsMatch(line))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }




    private bool ContainsCsFile(string directory)
        {
            try
            {
                var csFiles = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);
                return csFiles.Length > 0;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }



        private void btnCombine_Click(object sender, EventArgs e)
        {
            CombineSelectedFiles();
        }

        private void CombineSelectedFiles()
        {
            saveFileDialog.Filter = "All files (*.*)|*.*";
            saveFileDialog.FileName = "combined_file.txt";

            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                lastOutputFile = saveFileDialog.FileName;

                CombineInto(lastOutputFile);
            }
        }



        private void CombineInto(string targetFile) {
            using (var output = new FileStream(targetFile, FileMode.Create)) {
                foreach (TreeNode node in treeViewFiles.Nodes) {
                    FileSystemNode fileSystemNode = Utils.MapTreeNodeToFileSystemNode(node);
                    Core.CombineSelectedFilesRecursive(fileSystemNode, output, new FileSystem());
                }
            }

            lastOutputFile = targetFile;
            buttonCombineAgain.Enabled = true;

            MessageBox.Show("Files combined successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void button1_Click(object sender, EventArgs e) {
            btnCombine_Click(sender, e);
        }

        private void treeViewFiles_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                SetAllChildNodeCheckState(e.Node, e.Node.Checked);
            }
        }

        private void SetAllChildNodeCheckState(TreeNode node, bool checkState)
        {
            foreach (TreeNode childNode in node.Nodes)
            {
                childNode.Checked = checkState;
                SetAllChildNodeCheckState(childNode, checkState);
            }
        }

        private void buttonCombineAgain_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lastOutputFile))
            {
                CombineInto(lastOutputFile);
            }
            else
            {
                MessageBox.Show("Please use the 'Combine Selected Files' button first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
