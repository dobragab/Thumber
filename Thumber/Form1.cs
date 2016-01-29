using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Thumber
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thumb[] thumbs =
            {
                new Thumb("thumb", 300),
                new Thumb("mini", 235),
                new Thumb("micro", 155),
            };

            lbThumb.Items.AddRange(thumbs);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Thumb thumb = lbThumb.SelectedItem as Thumb;
            if (thumb == null)
                return;

            DialogResult result = ofd.ShowDialog();

            if (result != DialogResult.OK)
                return;

            progressBar1.Minimum = 1;
            progressBar1.Maximum = ofd.FileNames.Length;
            progressBar1.Step = 1;
            progressBar1.Value = 1;

            lbState.Text = String.Format("{0}/{1}", 0, ofd.FileNames.Length);
            lbState.Visible = true;

            string OutString = "";

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.WorkerReportsProgress = true;

            bgw.DoWork += delegate (object o, DoWorkEventArgs dwea)
            {
                for (int i = 0; i < ofd.FileNames.Length; ++i)
                {
                    string filename = ofd.FileNames[i];

                    string newfilename = Path.ChangeExtension(String.Format("{0}_{1}", Path.GetFileNameWithoutExtension(filename), thumb.name), Path.GetExtension(filename));
                    string newfile = Path.Combine(Path.GetDirectoryName(filename), newfilename);

                    if (File.Exists(newfile))
                    {
                        OutString += newfilename + " already exists. Skipping.";
                        continue;
                    }

                    string parameters = String.Format("-o \"{0}\" -overwrite -quiet -out jpeg -ratio -resize {2} 0 \"{1}\"", newfile, filename, thumb.width);
                    OutString += filename + " " + newfile + "\n";

                    ProcessStartInfo info = new ProcessStartInfo();
                    info.FileName = "nconvert.exe";
                    info.Arguments = parameters;
                    info.CreateNoWindow = true;
                    info.WindowStyle = ProcessWindowStyle.Hidden;

                    Process proc = Process.Start(info);
                    proc.WaitForExit();

                    bgw.ReportProgress(i);
                }
            };

            bgw.ProgressChanged += delegate (object o, ProgressChangedEventArgs pcea)
            {
                lbState.Text = String.Format("{0}/{1}", pcea.ProgressPercentage, ofd.FileNames.Length);
                progressBar1.PerformStep();
            };

            bgw.RunWorkerCompleted += delegate (object o, RunWorkerCompletedEventArgs rwcea)
            {
                progressBar1.Value = 1;
                lbState.Visible = false;
                this.Enabled = true;
                MessageBox.Show(OutString);
            };

            this.Enabled = false;
            
            bgw.RunWorkerAsync();


        }
    }
}
