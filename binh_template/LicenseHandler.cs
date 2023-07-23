using System;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Windows.Forms;

namespace binh_template
{
    class LicenseHandler
    {
        //----------------------- <BINH_HERE> -----------------------
        static IFirebaseClient client;
        static IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "YOUR_SECRET_FIREBASE_REALTIME_DATABASE_KEY",  //"abcdefgh"
            BasePath = @"YOUR_SECRET_FIREBASE_REALTIME_DATABASE_LINK"   //"https://....firebaseio.com/"
        };

        const string NODE_NAME = "MY_APP";
        //----------------------- </BINH_HERE> -----------------------



        private static void connectDatabase()
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                if (client == null)
                {
                    MessageBox.Show("Could not connect to database", "connectDatabase", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Exception {e.Message}", "connectDatabase", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }



        public static void checkLicense()
        {
            string license = Helper.LICENSE_CODE;
            string cpuID = Helper.CPU_ID;

            Console.WriteLine($"====> calling checkLicense({license}, {cpuID})");

            try
            {
                connectDatabase();

                if (license == "")
                {
                    verifyTrial(cpuID);
                }
                else
                {
                    verifyLicense(license, cpuID);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Exception {e.Message}", "checkLicense", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        } // checkLicense





        /*
         * **********************************************************************************
         * **********************************************************************************
         * ************************************** TRIAL *************************************
         * **********************************************************************************
         * **********************************************************************************
        */
        //Define a class to acquire CPU ID of the user when they start this application
        internal class DataTrial
        {
            public int counter { get; set; }
        }

        async static void verifyTrial(string cpuID)
        {
            Console.WriteLine($"====> calling verifyTrial({cpuID})");
            try
            {
                FirebaseResponse response = await client.GetTaskAsync(NODE_NAME + "/Trial/" + cpuID);
                Console.WriteLine("==> response.Body = " + response.Body);

                // If found the cpuID then check counter
                if (response.Body != "null")
                {
                    Console.WriteLine("==> checking counter");
                    DataTrial obj = response.ResultAs<DataTrial>();

                    if (obj.counter <= 0)
                    {
                        MessageBox.Show("You are out of trial times. Please purchase to continue using the application", "verifyTrial", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Application.Exit();
                    }
                }
                // regiter the cpuID (e.g first time installed)
                else
                {
                    Console.WriteLine("==> registering new cpuID");
                    try
                    {
                        var data = new DataTrial
                        {
                            counter = 5,
                        };

                        FirebaseResponse new_response = await client.SetTaskAsync(NODE_NAME + "/Trial/" + cpuID, data);
                        Console.WriteLine("==> new_response.Body = " + new_response.Body);
                        if (new_response.Body == "null")
                        {
                            MessageBox.Show("Failed registration", "verifyTrial", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Exception {e.Message}", "verifyTrial", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Exception {e.Message}", "verifyTrial", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }


        async public static void decreaseCounter()
        {
            string cpuID = Helper.getCpuId();

            Console.WriteLine($"====> calling decreaseCounter({cpuID})");
            try
            {
                FirebaseResponse response = await client.GetTaskAsync(NODE_NAME + "/Trial/" + cpuID);
                Console.WriteLine("==> response.Body = " + response.Body);

                // If found the cpuID then check counter
                if (response.Body != "null")
                {
                    Console.WriteLine("==> checking counter");
                    DataTrial obj = response.ResultAs<DataTrial>();

                    int res_counter = obj.counter;
                    if (res_counter <= 0)
                    {
                        MessageBox.Show("You are out of trial times. Please purchase to continue using the application", "decreaseCounter", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Application.Exit();
                    }

                    res_counter = res_counter - 1;
                    try
                    {
                        var data = new DataTrial
                        {
                            counter = res_counter,
                        };

                        FirebaseResponse new_response = await client.SetTaskAsync(NODE_NAME + "/Trial/" + cpuID, data);
                        Console.WriteLine("==> new_response.Body = " + new_response.Body);
                        if (new_response.Body == "null")
                        {
                            MessageBox.Show("Failed decrease counter", "decreaseCounter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Exception {e.Message}", "decreaseCounter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Exception {e.Message}", "decreaseCounter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }




        /*
         * **********************************************************************************
         * **********************************************************************************
         * ************************************** VIP *************************************
         * **********************************************************************************
         * **********************************************************************************
        */
        //Define a class to acquire CPU ID of the user when they start this application
        internal class DataVip
        {
            public string cpuID { get; set; }
        }

        //This function is used to check the user's license if it is matched with their CPU id which is registered at the first time they start the app
        async public static void verifyLicense(string license, string cpuID)
        {
            Console.WriteLine($"====> calling verifyLicense({license},{cpuID})");
            try
            {
                connectDatabase();
                FirebaseResponse response = await client.GetTaskAsync(NODE_NAME + "/Vip/" + license);
                Console.WriteLine("==> response.Body = " + response.Body);

                if (response.Body != "null")
                {
                    DataVip obj = response.ResultAs<DataVip>();

                    //"00" means new license is not activated and can be used
                    if (obj.cpuID == "00")
                    {
                        activateLicense(license, cpuID);
                    }
                    else if (obj.cpuID == cpuID)
                    {
                        Console.WriteLine("License OK, Happy Welcome Back ❤️");
                        //MessageBox.Show("License OK, Happy Welcome Back ❤️", "verifyLicense", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (obj.cpuID != cpuID)
                    {
                        MessageBox.Show($"This license '{license}' was used. Please purchase a new license", "verifyLicense", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
                    else
                    {
                        MessageBox.Show("Please purchase a new license", "verifyLicense", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
                }
                else
                {
                    MessageBox.Show($"The license {license}' does not exist", "verifyLicense", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Exception {e.Message}", "verifyLicense", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        } // verifyLicense


        //This function is used to instal license to the current user's CPU id
        async static void activateLicense(string license, string cpuInfo)
        {
            Console.WriteLine($"====> calling activateLicense({license},{cpuInfo})");
            try
            {
                var data = new DataVip
                {
                    cpuID = cpuInfo,
                };

                FirebaseResponse response = await client.SetTaskAsync(NODE_NAME + "/Vip/" + license, data);
                Console.WriteLine("==> response.Body = " + response.Body);
                if (response.Body != "null")
                {
                    MessageBox.Show("Activated license sucessfully !", "activateLicense", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Helper.reCreatSettingFile(license, Helper.USER_NAME, Helper.USER_PASSWORD);
                    Helper.reloadSettingFile();
                }
                else
                {
                    MessageBox.Show("Failed activation", "activateLicense", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Exception {e.Message}", "activateLicense", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }// activateLicense

    }
}
