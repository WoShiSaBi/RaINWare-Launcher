using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using MetroFramework;
using MetroFramework.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApplication1
{
    public partial class Form2 : MetroForm
    {
        public Form2()
        {
            InitializeComponent();

            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // #1
            DoesFileExist();
        }

        private void injectbutton_Click(object sender, EventArgs e)
        {
            if (metroComboBox1.SelectedItem == "Skinchanger")
                InjectSkinchanger();
            else
                MetroMessageBox.Show(this, "Select a Product First!", "Error!", MessageBoxButtons.OK,
                    MessageBoxIcon.Question);
        }

        private void InjectSkinchanger()
        {
            var fullpath2 = "C:\\Users\\" + Environment.UserName +
                            "\\AppData\\Local\\Temp\\Report.10LAJ149-0OQC-K3IS1-95C1-344D9BE7PQU4\\EB3O294E-FC87-453F-BBFD-24137FOQH3EE\\104D2149-016C-46A9-95C1-344D8E870104.m.etl";

            if (File.Exists(fullpath2))
            {
                var pname = Process.GetProcessesByName("csgo");

                if (pname.Length == 0)
                {
                    var result = MetroMessageBox.Show(this, "CSGO is not Running!\r\nDo you want to start it?",
                        "Information", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);

                    if (result == DialogResult.Yes)
                        try
                        {
                            Process.Start("steam://rungameid/730");
                        }
                        catch (Exception)
                        {
                            MetroMessageBox.Show(this, "Failed to start CSGO", "Error!", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                }
                else
                {
                    try
                    {
                        var strDLLName = fullpath2;
                        var strProcessName = "csgo";

                        var ProcID = GetProcessId(strProcessName);
                        if (ProcID >= 0)
                        {
                            var hProcess = OpenProcess(0x1F0FFF, 1, ProcID);
                            if (hProcess == null)
                            {
                                MessageBox.Show("OpenProcess() Failed!");
                            }
                            else
                            {
                                InjectDLL(hProcess, strDLLName);
                                Console.Beep();
                                Close();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MetroMessageBox.Show(this, "Error while trying to Inject into CSGO!\r\nProgramm will now close!",
                            "Information", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        Close();
                    }
                }
            }
            else
            {
                Process.Start("Updater.exe");
                Close();
            }
        }

        // #1
        private void DoesFileExist()
        {
            var fullpath = "C:\\Users\\" + Environment.UserName +
                           "\\AppData\\Local\\Temp\\Report.10LAJ149-0OQC-K3IS1-95C1-344D9BE7PQU4";
            var fullpath2 = "C:\\Users\\" + Environment.UserName +
                            "\\AppData\\Local\\Temp\\Report.10LAJ149-0OQC-K3IS1-95C1-344D9BE7PQU4\\EB3O294E-FC87-453F-BBFD-24137FOQH3EE";

            var exists = Directory.Exists(fullpath);
            var exists2 = Directory.Exists(fullpath2);

            // Download Path Exists
            if (exists)
            {
                if (exists2)
                    if (File.Exists(fullpath2 + "\\104D2149-016C-46A9-95C1-344D8E870104.m.etl"))
                        backgroundWorker1.RunWorkerAsync();
                    else
                        DownloadFile2("HACKDLLDOWNLOADHERE",
                            fullpath2 + "\\104D2149-016C-46A9-95C1-344D8E870104.m.etl");
            }
            else
            {
                // Download Path does not Exists, Create & Download File
                Directory.CreateDirectory(fullpath);

                if (!exists2)
                {
                    Directory.CreateDirectory(fullpath2);

                    if (
                        File.Exists("C:\\Users\\" + Environment.UserName +
                                    "\\AppData\\Local\\Temp\\Report.10LAJ149-0OQC-K3IS1-95C1-344D9BE7PQU4\\EB3O294E-FC87-453F-BBFD-24137FOQH3EE\\104D2149-016C-46A9-95C1-344D8E870104.m.etl"))
                        File.Delete("C:\\Users\\" + Environment.UserName +
                                    "\\AppData\\Local\\Temp\\Report.10LAJ149-0OQC-K3IS1-95C1-344D9BE7PQU4\\EB3O294E-FC87-453F-BBFD-24137FOQH3EE\\104D2149-016C-46A9-95C1-344D8E870104.m.etl");

                    DownloadFile2("HACKDLLDOWNLOADHERE",
                        fullpath2 + "\\104D2149-016C-46A9-95C1-344D8E870104.m.etl");
                }
            }
        }

        private void DownloadFile2(string DownloadURL, string savePath)
        {
            WebClient webClient;

            using (webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += Completed2;
                webClient.DownloadProgressChanged += ProgressChanged2;

                var URL = DownloadURL.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                    ? new Uri(DownloadURL)
                    : new Uri("http://" + DownloadURL);

                try
                {
                    webClient.DownloadFileAsync(URL, savePath);
                }
                catch (Exception)
                {
                    MetroMessageBox.Show(this, "\nDownloading Update Failed", "Error!", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void ProgressChanged2(object sender, DownloadProgressChangedEventArgs e)
        {
            metroProgressSpinner1.Value = e.ProgressPercentage;

            metroLabel2.Text = "Downloading: " + e.ProgressPercentage + "%";
        }

        // After Download Completion
        private void Completed2(object sender, AsyncCompletedEventArgs e)
        {
            metroLabel2.Text = "Ready to Inject!";
            injectbutton.Enabled = true;
        }

        #region Update Function

        //Connection String
        private static readonly string MyConnectionString = "Server=SERVERHERE;" +
                                                            "Database=DATABASENAMEHERE;" +
                                                            "Uid=DATABASEUSERHERE;" +
                                                            "Pwd=DATABASEPASSWORDHERE;";

        // #2
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(10);

            var connection = new MySqlConnection(MyConnectionString);
            try
            {
                backgroundWorker1.ReportProgress(35);

                //Remote VersionCheck Function 
                var doc = new XmlDocument();
                doc.Load("VERSIONCHECK.XML LINK HERE");

                var node = doc.DocumentElement.SelectSingleNode("/VersionCheck/SkinChanger/Version");

                var SkinchangerVersion = node.InnerText;

                try
                {
                    backgroundWorker1.ReportProgress(65);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    // Dont forget to rename "LauncherVersion" to "SkinchangerVersion"
                    var VerifyVersion =
                        new MySqlCommand("SELECT count(*) from Information where (LauncherVersion = @VERSION)",
                            connection);
                    VerifyVersion.Parameters.AddWithValue("@VERSION", SkinchangerVersion);

                    var CheckingVersion = (int) (long) VerifyVersion.ExecuteScalar();

                    if (CheckingVersion >= 1)
                        backgroundWorker1.ReportProgress(90);
                    else
                        try
                        {
                            backgroundWorker1.ReportProgress(100);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            metroProgressSpinner1.Value = e.ProgressPercentage;

            if (metroProgressSpinner1.Value >= 1)
                metroLabel2.Text = "Connecting...";

            if (metroProgressSpinner1.Value >= 10)
                metroLabel2.Text = "Searching for Updates...";

            if (metroProgressSpinner1.Value >= 35)
                metroLabel2.Text = "Connecting to Database...";

            if (metroProgressSpinner1.Value >= 65)
                metroLabel2.Text = "Checking Hack Version...";
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                metroLabel2.Text = "Process was cancelled";
            }
            else if (e.Error != null)
            {
                metroLabel2.Text = "There was an error running the process. The thread aborted";
            }
            else if (metroProgressSpinner1.Value == 10)
            {
                metroLabel2.Text = "Searching Update Failed!";
            }
            else if (metroProgressSpinner1.Value == 35)
            {
                metroLabel2.Text = "Cant Connect to Database!";
            }
            else if (metroProgressSpinner1.Value == 65)
            {
                metroLabel2.Text = "Cant get Launcher Version!";
            }
            else if (metroProgressSpinner1.Value == 90)
            {
                metroLabel2.Text = "Ready to Inject!";
                injectbutton.Enabled = true;
            }
            else if (metroProgressSpinner1.Value == 100)
            {
                metroLabel2.Text = "Update Found!";

                var Updating = MetroMessageBox.Show(this, "", "Update Available!", MessageBoxButtons.OK,
                    MessageBoxIcon.Question);

                if (Updating == DialogResult.OK)
                {
                    var fullpath2 = "C:\\Users\\" + Environment.UserName +
                                    "\\AppData\\Local\\Temp\\Report.10LAJ149-0OQC-K3IS1-95C1-344D9BE7PQU4\\EB3O294E-FC87-453F-BBFD-24137FOQH3EE";

                    if (File.Exists(fullpath2 + "\\104D2149-016C-46A9-95C1-344D8E870104.m.etl"))
                        File.Delete(fullpath2 + "\\104D2149-016C-46A9-95C1-344D8E870104.m.etl");

                    DownloadFile2("HACKDLLDOWNLOADHERE",
                        fullpath2 + "\\104D2149-016C-46A9-95C1-344D8E870104.m.etl");
                }
            }
        }

        #endregion

        #region Inject Function

        [DllImport("kernel32")]
        public static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttributes,
            uint dwStackSize,
            UIntPtr lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            out IntPtr lpThreadId
        );

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
            uint dwDesiredAccess,
            int bInheritHandle,
            int dwProcessId
        );

        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(
            IntPtr hObject
        );

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool VirtualFreeEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            UIntPtr dwSize,
            uint dwFreeType
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern UIntPtr GetProcAddress(
            IntPtr hModule,
            string procName
        );

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr VirtualAllocEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            uint dwSize,
            uint flAllocationType,
            uint flProtect
        );

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            string lpBuffer,
            UIntPtr nSize,
            out IntPtr lpNumberOfBytesWritten
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(
            string lpModuleName
        );

        [DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
        internal static extern int WaitForSingleObject(
            IntPtr handle,
            int milliseconds
        );

        public int GetProcessId(string proc)
        {
            Process[] ProcList;
            ProcList = Process.GetProcessesByName(proc);
            return ProcList[0].Id;
        }

        public void InjectDLL(IntPtr hProcess, string strDLLName)
        {
            IntPtr bytesout;

            var LenWrite = strDLLName.Length + 1;
            var AllocMem = VirtualAllocEx(hProcess, (IntPtr) null, (uint) LenWrite, 0x1000, 0x40);

            WriteProcessMemory(hProcess, AllocMem, strDLLName, (UIntPtr) LenWrite, out bytesout);
            var Injector = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            if (Injector == null)
            {
                MessageBox.Show("Injector Error!");
                return;
            }

            var hThread =
                CreateRemoteThread(hProcess, (IntPtr) null, 0, Injector, AllocMem, 0, out bytesout);
            if (hThread == null)
            {
                MessageBox.Show("hThread[1] Error!");
                return;
            }
            var Result = WaitForSingleObject(hThread, 10*1000);
            if ((Result == 0x00000080L) || (Result == 0x00000102L) || (Result == 0xFFFFFFFF))
            {
                MessageBox.Show("hThread[2] Error!");
                if (hThread != null)
                    CloseHandle(hThread);
                return;
            }
            Thread.Sleep(1000);
            VirtualFreeEx(hProcess, AllocMem, (UIntPtr) 0, 0x8000);
            if (hThread != null)
                CloseHandle(hThread);
        }

        #endregion Inject Function
    }
}