using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;
using System.Windows.Forms;

namespace binh_template
{
    class VersionHandler
    {
        public static void checkVersion()
        {
            //----------------------- <BINH_HERE> -----------------------
            const string VERSION = "1.0.0";
            const string APP_NAME = "MyApp";
            // ** NOTE ** replace at the end of the link "dl=0" with "dl=1" (auto download)
            const string TXT_LINK = "YOUR_LINK_TO_WEBHOST_UPDATE_TEXT_FILE"; //e.g "https://www.dropbox.com/scl/fi/cc7f9pbbcfqbrf1myvb6h/Update.txt?...&dl=1"
            const string ZIP_LINK = "YOUR_LINK_TO_WEBHOST_ZIP_MSI_FILE";     //e.g "https://www.dropbox.com/scl/fi/uidhpgb21l7dqdhk0vvoz/MyAppSetup.zip?...&dl=1"
            string MSI_FILE = $@"{APP_NAME}Setup.msi";
            string MSI_PATH = @".\" + MSI_FILE;
            string ZIP_FILE = $@"{APP_NAME}Setup.zip";
            string ZIP_PATH = @".\" + ZIP_FILE;
            //----------------------- </BINH_HERE> -----------------------


            WebClient webClient = new WebClient();
            var client = new WebClient();

            string serverName = webClient.DownloadString(TXT_LINK).Between("<NAME>", "</NAME>");
            string serverVersion = webClient.DownloadString(TXT_LINK).Between("<VERSION>", "</VERSION>");
            string serverForce = webClient.DownloadString(TXT_LINK).Between("<FORCE>", "</FORCE>");
            string serverInfo = webClient.DownloadString(TXT_LINK).Between("<INFO>", "</INFO>");

            if (serverVersion != VERSION)
            {
                if (MessageBox.Show($"Current version : {VERSION}\nAvailable version : {serverVersion}\n" + serverInfo, serverName, MessageBoxButtons.YesNo, (serverForce == "TRUE") ? MessageBoxIcon.Warning : MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        if (File.Exists(MSI_PATH))
                        {
                            File.Delete(MSI_PATH);
                        }
                        client.DownloadFile(ZIP_LINK, ZIP_FILE);
                        string zipPath = ZIP_PATH;
                        string extractPath = @".\";
                        ZipFile.ExtractToDirectory(zipPath, extractPath);
                        Process process = new Process();
                        process.StartInfo.FileName = "msiexec";
                        process.StartInfo.Arguments = String.Format("/i " + MSI_FILE);
                        //this.Close();
                        process.Start();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Exception {e.Message}", "checkVersion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if (serverForce == "TRUE")
                    {
                        Application.Exit();
                    }
                }
            }
        }
    }
}