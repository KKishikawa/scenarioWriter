using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace scenarioWriter
{
    public partial class frmMain : Form
    {
        private bool isEdited = false;
        private string filePath = string.Empty;
        public frmMain()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        private void openStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // operation
        private void NewFile()
        {
            if (isEdited)
            {
                var msgTxt = new StringBuilder();
                msgTxt.Append("File has been changed.").AppendLine()
                      .Append("Do you want to save changes?");

                var dialogRet = MessageBox.Show(msgTxt.ToString(), "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                switch (dialogRet)
                {
                    case DialogResult.Yes:
                        SaveFile();
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }
            this.filePath = string.Empty;
            textBox1.Text = string.Empty;
            isEdited = false;
        }

        private void SaveFile()
        {
            if(filePath != string.Empty)
            {
                SaveFile(filePath);
            }
            else
            {
                SaveFileAs();
            }
        }

        private void SaveFileAs()
        {
            using(var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "txt files (*.txt)|*.txt";
                saveFileDialog.RestoreDirectory = true;
                if(saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = saveFileDialog.FileName;
                    SaveFile(filePath);
                }
            }
        }

        private void SaveFile(string filePath)
        {

            try
            {
                File.WriteAllText(filePath, textBox1.Text);
                isEdited = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void OpenFile()
        {
            using(var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "txt files (*.txt)|*.txt";
                openFileDialog.RestoreDirectory = true;
                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = openFileDialog.FileName;
                    LoadFile(filePath);
                }
            }
        }

        private void LoadFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                var msgTxt = new StringBuilder();
                msgTxt.Append("Cannot Find a File!");
                MessageBox.Show(msgTxt.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            StreamReader streamReader = null;
            try
            {
                streamReader = File.OpenText(filePath);
                textBox1.Text = streamReader.ReadToEnd();
                this.filePath = filePath;
                isEdited = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                throw;
            }
            finally
            {
                if(streamReader != null)
                {
                    streamReader.Close();
                }
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isEdited)
            {
                var msgTxt = new StringBuilder();
                msgTxt.Append("File has been changed.").AppendLine()
                      .Append("Do you want to save changes?");

                var dialogRet = MessageBox.Show(msgTxt.ToString(), "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                switch (dialogRet)
                {
                    case DialogResult.Yes:
                        SaveFile();
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] drags = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (drags.Length != 1) return;
                if (Path.GetExtension(drags[0]) != ".txt") return;
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void frmMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] drags = (string[])e.Data.GetData(DataFormats.FileDrop);
            LoadFile(drags[0]);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!isEdited) isEdited = true;
        }
    }
}
