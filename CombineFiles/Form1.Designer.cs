namespace CombineFiles
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.treeViewFiles = new System.Windows.Forms.TreeView();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.buttonCombineAgain = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(541, 834);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(198, 50);
            this.button1.TabIndex = 0;
            this.button1.Text = "Combine selected files";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // treeViewFiles
            // 
            this.treeViewFiles.CheckBoxes = true;
            this.treeViewFiles.Dock = System.Windows.Forms.DockStyle.Top;
            this.treeViewFiles.Location = new System.Drawing.Point(0, 0);
            this.treeViewFiles.Name = "treeViewFiles";
            this.treeViewFiles.Size = new System.Drawing.Size(780, 816);
            this.treeViewFiles.TabIndex = 1;
            this.treeViewFiles.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewFiles_AfterCheck);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.InitialDirectory = "c:\\\\temp";
            // 
            // buttonCombineAgain
            // 
            this.buttonCombineAgain.Enabled = false;
            this.buttonCombineAgain.Location = new System.Drawing.Point(291, 834);
            this.buttonCombineAgain.Name = "buttonCombineAgain";
            this.buttonCombineAgain.Size = new System.Drawing.Size(228, 50);
            this.buttonCombineAgain.TabIndex = 2;
            this.buttonCombineAgain.Text = "Combine Again";
            this.buttonCombineAgain.UseVisualStyleBackColor = true;
            this.buttonCombineAgain.Click += new System.EventHandler(this.buttonCombineAgain_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 905);
            this.Controls.Add(this.buttonCombineAgain);
            this.Controls.Add(this.treeViewFiles);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Combine selected files";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TreeView treeViewFiles;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button buttonCombineAgain;
    }
}

