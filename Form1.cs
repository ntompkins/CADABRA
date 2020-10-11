using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;

namespace cadabra
{
    public partial class Form1 : Form
    {
        // TODO: Don't do this
        private string[] workingDirectoryFiles = new string[] { };

        public Form1()
        {
            InitializeComponent();
        }

        private void CurrentDirectoryDisplay_update(object path)
        {
            CurrentDirectoryDisplay.Text = path.ToString();
        }

        // Click event to open a directory
        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {

                    // Store full file paths
                    workingDirectoryFiles = Directory.GetFiles(fbd.SelectedPath);

                    // Update display with dir contents
                    string[] fileNames = GetFileNames(fbd.SelectedPath); 

                    // Update window contents
                    FileBrowserDisplay_update(fileNames);

                    // Update CWD
                    CurrentDirectoryDisplay_update(fbd.SelectedPath);
                }
            }
        }

        // Click event to unzip a zip file
        private void openZipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                // Get input zip file
                DialogResult result = ofd.ShowDialog();

                // Verify file exists
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
                {
                    // Verify file is a zip
                    if (Path.GetExtension(ofd.FileName).ToLower() == @".zip")
                    {
                        // Replace file name with directory name
                        string unzipLocation = ofd.FileName.Replace(ofd.SafeFileName, Path.GetFileNameWithoutExtension(ofd.FileName));

                        // Extract file
                        try
                        {
                            ZipFile.ExtractToDirectory(ofd.FileName, unzipLocation);
                        } catch (System.IO.IOException err)
                        {
                            System.Windows.Forms.MessageBox.Show("Error, unzip directory already exists.");
                            Console.WriteLine(err.ToString());
                        }

                        // Store full file paths
                        workingDirectoryFiles = Directory.GetFiles(unzipLocation);

                        // Update file browser with just filenames
                        string[] fileNames = GetFileNames(unzipLocation);
                        // Update window contents
                        FileBrowserDisplay_update(fileNames);
                        // Update CWD
                        CurrentDirectoryDisplay_update(unzipLocation);

                    } else
                    {
                        System.Windows.Forms.MessageBox.Show("Error, file is not a zip.");
                    }
                }
            }
        }

        private void FileBrowserDisplay_update(string[] fileList)
        {
            // Always start from scratch
            FileBrowserDisplay.Items.Clear();

            // Update listbox with contents of array
            for (int i = 0; i < fileList.Length; i++)
            {
                FileBrowserDisplay.Items.Add(fileList[i]);
            }
        }

        // STANDALONE METHODS
        private string[] GetFileNames(string curDir)//, string[] tempList)
        {
            // Always clear temp list
            // TODO: Add regex to filter only solidwork files and update method's name
            return Directory.GetFiles(curDir).Select(Path.GetFileName).ToArray();
        }

        private string CalculateSHA(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            using (BufferedStream bs = new BufferedStream(fs))
            using (var crypto = new SHA1CryptoServiceProvider())
            {
                return BitConverter.ToString(crypto.ComputeHash(bs));
            }
        }

        private bool QuickVerifySHAS(string[] SHAs)
        {
            return SHAs.Distinct().Count() < SHAs.Length;
        }

        private void ScanButton_Click(object sender, EventArgs e)
        {
            // List to store SHA sums
            string[] SHAList = new string[workingDirectoryFiles.Length];

            // Calculate SHA for each file in curdir
            for (int i = 0; i < workingDirectoryFiles.Length; i++) 
            {
                SHAList[i] = CalculateSHA(workingDirectoryFiles[i]);
            }

            // Output results of scan
            if (QuickVerifySHAS(SHAList))
            {
                System.Windows.Forms.MessageBox.Show("Uh OH! Someone cheated.");
            } else
            {
                System.Windows.Forms.MessageBox.Show("Yippee, they got away with it.");
            }
            
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
