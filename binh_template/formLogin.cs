using System;
using System.IO;
using System.Windows.Forms;

// BINH_NOTE : Choose which form is load firstly in "Program.cs"

namespace binh_template
{
    public partial class formLogin : Form
    {
        public formLogin()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(Helper.CreateRoundRectRgn(0, 0, Width, Height, 25, 25)); // bo tròn 4 góc
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void formLogin_Load(object sender, EventArgs e)
        {
            // if not exist (first time installed), create it
            if (!File.Exists(Helper.SETTING_FILE))
            {
                Helper.reCreatSettingFile("", "", "");
            }

            Helper.reloadSettingFile();
            textBoxName.Text = Helper.USER_NAME;
            textBoxPassword.Text = Helper.USER_PASSWORD;
        }


        private void pictureBoxLicense_Click(object sender, EventArgs e)
        {
            string cpuID = Helper.getCpuId();
            string license = Microsoft.VisualBasic.Interaction.InputBox($"Please enter your license code\n\nYour id         : {cpuID}\nYour license : {Helper.LICENSE_CODE}", "License", Helper.LICENSE_CODE);

            if (license != "" && license != Helper.LICENSE_CODE && File.Exists(Helper.SETTING_FILE))
            {
                LicenseHandler.verifyLicense(license, cpuID);
            }
        }



        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Step 1 : checking user + password
            if (textBoxName.Text != "admin" && textBoxPassword.Text != "admin")
            {
                MessageBox.Show("Wrong user password !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                Helper.reCreatSettingFile(Helper.LICENSE_CODE, textBoxName.Text, textBoxPassword.Text);
                Helper.reloadSettingFile();
            }
            progressBar1.Value = 50;

            // Step 2 : checking license
            labelInfo.Text = "Checking license...";
            LicenseHandler.checkLicense();
            progressBar1.Value = 70;

            // Step 3 : checking version
            labelInfo.Text = "Checking version...";
            VersionHandler.checkVersion();
            progressBar1.Value = 100;

            // Step 4 : Access to main page
            this.Hide();
            formMain fm = new formMain();
            fm.Show();
        }


    }
}
