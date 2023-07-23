using System;
using System.Globalization;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace binh_template
{
    class Helper
    {
        public static string SETTING_FILE = System.IO.Directory.GetCurrentDirectory() + "\\setting.txt";
        public static string LICENSE_CODE = "";
        public static string USER_NAME = "";
        public static string USER_PASSWORD = "";
        public static string CPU_ID = getCpuId();


        //Dùng để bo tròn 4 góc màn hình
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn
        (
               int nLeftRect,
               int nTopRect,
               int nRightRect,
               int nBottomRect,
               int nWidthEllipse,
               int nHeightEllipse

        );


        public static string newReference(string prefix)
        {
            string token = prefix + DateTime.Now.Ticks.ToString().Substring(14, 2) + DateTime.Now.Ticks.ToString().Substring(3, 1) + DateTime.Now.Ticks.ToString().Substring(4, 2);
            return token;
        }

        public static string nvl(string a, string b)
        {
            if(a != "")
            {
                return a;
            }
            return b;
        }

        public static void reCreatSettingFile(string license, string user, string password)
        {
            Console.WriteLine("====> calling reCreatSettingFile()");
            if (File.Exists(SETTING_FILE))
            {
                File.Delete(SETTING_FILE);
            }

            StreamWriter newFile = new StreamWriter(SETTING_FILE);
            newFile.WriteLine($@"<LICENSE>{license}</LICENSE>");
            newFile.WriteLine($@"<USER>{user}</USER>");
            newFile.WriteLine($@"<PASSWORD>{password}</PASSWORD>");
            newFile.Close();
        }

        public static void reloadSettingFile()
        {
            try
            {
                string[] lines = File.ReadAllLines(SETTING_FILE);
                LICENSE_CODE = lines[0].Between("<LICENSE>", "</LICENSE>");
                USER_NAME = lines[1].Between("<USER>", "</USER>");
                USER_PASSWORD = lines[2].Between("<PASSWORD>", "</PASSWORD>");
            }
            catch (Exception e)
            {
                MessageBox.Show($@"Missing {SETTING_FILE} {e.Message}", "reloadSettingFile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        static public string getCpuId()
        {
            //Get the unique CPU id
            string cpuID = string.Empty;

            try
            {
                ManagementClass mc = new ManagementClass("win32_processor");
                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if (cpuID == "")
                    {
                        //Get only the first CPU id
                        cpuID = mo.Properties["processorID"].Value.ToString();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Exception {e.Message}", "getCpuId", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            if (cpuID == "")
            {
                MessageBox.Show("Could not find id", "getCpuId", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            Console.WriteLine($"==> cpuID = {cpuID}");
            return cpuID;
        } // getCpuId
    }


    static class SubstringExtensions
    {
        public static string RemoveDiacritics(this string text)
        {
            // BINH : fix issue with "đ", can't search "cá đuối"
            string mytext = text.Replace("đ", "d");
            var normalizedString = mytext.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }


        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
        public static string Between(this string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Get string value after [first] a.
        /// </summary>
        public static string Before(this string value, string a)
        {
            int posA = value.IndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            return value.Substring(0, posA);
        }

        /// <summary>
        /// Get string value after [last] a.
        /// </summary>
        public static string After(this string value, string a)
        {
            int posA = value.LastIndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }
    }
}
