using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Downloader
{
    public partial class Form1 : Form
    {
        static private WebClient client;
        static private Uri url;
        public Form1()
        {
            InitializeComponent();
            client = new WebClient();

            client.Disposed +=
            (s, e) =>
            {
                DownloadBar.Value = 0;
                client.CancelAsync();
                while (client.IsBusy) ;
                try
                {
                    if (File.Exists(this.tbFile.Text))
                        File.Delete(this.tbFile.Text);
                }
                catch(Exception ex)
                {
                    /*FAIL!*/
                }
                this.btBrowse.Enabled = true;
                this.btDownload.Enabled = true;
                this.btCancel.Enabled = false;
            };

            client.DownloadDataCompleted += 
            (s, e) =>
            {
                DownloadBar.Value = 0;
                this.btBrowse.Enabled = true;
                this.btDownload.Enabled = true;
                this.btCancel.Enabled = false;
                this.Refresh();
            };

            client.DownloadProgressChanged +=
            (s, e) =>
            {
                DownloadBar.Value = e.ProgressPercentage;
                this.Refresh();
            };
        }

        private void btBrowse_Click(object sender, EventArgs e)
        {
            if(!tbUrl.Text.Equals(""))
            {
                url = new Uri(tbUrl.Text);
                saveFileDialog.FileName = Path.
                    GetFileName(url.AbsolutePath);
                saveFileDialog.AddExtension = true;
            }
            saveFileDialog.ShowDialog();
        }

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            this.tbFile.Text = saveFileDialog.FileName;
        }

        private void btDownload_Click(object sender, EventArgs e)
        {
            try 
            {
                this.btBrowse.Enabled = false;
                this.btDownload.Enabled = false;
                this.btCancel.Enabled = true;
                client.DownloadFileAsync(url, this.tbFile.Text);
            }
            catch(Exception ex)
            {
                client.Dispose();
                MessageBox.Show(this, ex.ToString());
            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            client.Dispose();
            this.btCancel.Enabled = false;
        }
    }
}
