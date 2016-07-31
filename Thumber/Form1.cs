using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Threading;
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

        private void ProgressInit(int max)
        {
            progressBar1.Maximum = max;

            lbState.Text = String.Format("{0}/{1}", 0, max);
            lbState.Visible = true;

            this.Enabled = false;
        }

        private void ProgressStep(int max, int current)
        {
            progressBar1.PerformStepNoAnimation();
            lbState.Text = String.Format("{0}/{1}", current, max);
        }

        private void ProgressFinish()
        {
            progressBar1.Value = 0;
            lbState.Visible = false;
            this.Enabled = true;
        }

        private void ConvertFiles(string[] files, Thumb thumb)
        {
            ProgressInit(files.Length);

            string OutString = "";

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.WorkerReportsProgress = true;

            bgw.DoWork += delegate (object o, DoWorkEventArgs dwea)
            {
                for (int i = 0; i < files.Length; ++i)
                {
                    try
                    {
                        ConvertParams oldparams = ConvertParams.GetParams(files[i]);
                        ConvertParams newparams = ConvertParams.GetConvertedParams(oldparams, thumb);

                        if(!ConvertParams.ConvertFile(oldparams, newparams, thumb))
                            OutString += newparams.filename + " already exists. Skipping.\n";

                        bgw.ReportProgress(i+1);
                    }
                    catch (Exception x)
                    {
                        OutString += x.Message + "\n" + x.StackTrace + "\n";
                    }
                }
            };

            bgw.ProgressChanged += delegate (object o, ProgressChangedEventArgs pcea)
            {
                ProgressStep(ofd.FileNames.Length, pcea.ProgressPercentage);
            };

            bgw.RunWorkerCompleted += delegate (object o, RunWorkerCompletedEventArgs rwcea)
            {
                OutString += "Completed.";
                MessageBox.Show(OutString);
                ProgressFinish();
            };

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
