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
                new Thumb("logout", 611),   // logout // or 628?
                new Thumb("forum", 480),    // forum
                new Thumb("thumb", 300),    // one only
                new Thumb("mini", 235),     // 2
                new Thumb("micro", 155),    // 3
                new Thumb("nano", 115),     // 4
                new Thumb("pico", 90),      // 5
                new Thumb("femto", 75),     // 6
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

            ConvertFiles(ofd.FileNames, thumb);
        }

        private void ConvertFiles(string[] files, Thumb thumb)
        {
            progressBar1.Minimum = 1;
            progressBar1.Maximum = files.Length;
            progressBar1.Step = 1;
            progressBar1.Value = 1;

            lbState.Text = String.Format("{0}/{1}", 0, files.Length);
            lbState.Visible = true;

            string OutString = "";

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.WorkerReportsProgress = true;

            bgw.DoWork += delegate (object o, DoWorkEventArgs dwea)
            {
                for (int i = 0; i < files.Length; ++i)
                {
                    try
                    {
                        string filename = files[i];
                        string ext = Path.GetExtension(filename);
                        bool png = ext.ToUpper() == ".PNG";

                        string newfilename = Path.ChangeExtension(String.Format("{0}_{1}", Path.GetFileNameWithoutExtension(filename), thumb.name), ext);
                        string newfile = Path.Combine(Path.GetDirectoryName(filename), newfilename);

                        if (File.Exists(newfile))
                        {
                            OutString += newfilename + " already exists. Skipping.";
                            continue;
                        }

                        string parameters = String.Format("-o \"{0}\" -overwrite -quiet -out {3} -ratio -resize {2} 0 \"{1}\"", newfile, filename, thumb.width, png ? "png" : "jpeg");

                        ProcessStartInfo info = new ProcessStartInfo();
                        info.FileName = "nconvert.exe";
                        info.Arguments = parameters;
                        info.CreateNoWindow = true;
                        info.WindowStyle = ProcessWindowStyle.Hidden;

                        Process proc = Process.Start(info);
                        proc.WaitForExit();

                        bgw.ReportProgress(i);
                    }
                    catch (Exception x)
                    {
                        OutString += x.Message + "\n" + x.StackTrace + "\n";
                    }
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
                OutString += "Completed.";
                MessageBox.Show(OutString);
            };

            this.Enabled = false;

            bgw.RunWorkerAsync();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.HasFiles())
                e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            Thumb thumb = lbThumb.SelectedItem as Thumb;
            if (thumb == null)
                return;

            ConvertFiles(e.GetFiles(), thumb);
        }

        private void lbThumb_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            btnOK_Click(sender, e);
        }
    }
}
