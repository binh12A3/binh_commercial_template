using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace binh_template
{
    public partial class formMain : Form
    {
        public formMain()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(Helper.CreateRoundRectRgn(0, 0, Width, Height, 25, 25)); // bo tròn 4 góc
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg_im = new OpenFileDialog();
                //dlg_im.Filter = "Excel File|*.xls;*.xlsx;*.xlsm";
                //dlg_im.Filter = "Text|*.txt|All|*.*";

                if (dlg_im.ShowDialog() == DialogResult.OK)
                {
                    textBoxPath.Text = dlg_im.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = Environment.CurrentDirectory;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            string newRef = Helper.newReference("ID");
            textBox1.Text = newRef;

            textBoxPath.Text = System.IO.Directory.GetCurrentDirectory();

            string str = "AnhYeuEm";
            Console.WriteLine("==>" + str.Between("Anh", "Em"));

            progressBar1.Value = 50;

            if(Helper.LICENSE_CODE == "")
            {
                LicenseHandler.decreaseCounter();
            }

            MessageBox.Show("Done");
        }

        private void pictureBoxLogout_Click(object sender, EventArgs e)
        {
            Helper.reCreatSettingFile(Helper.LICENSE_CODE, "", "");
            Helper.reloadSettingFile();
            Application.Exit();
        }
    }
}
