using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;
using MySql.Data.MySqlClient;
using static WindowsFormsApplication1.HWID;


namespace WindowsFormsApplication1
{
    public partial class Form1 : MetroForm
    {
        //Connection String
        private static readonly string MyConnectionString = "Server=SERVERHERE;" +
                                                            "Database=DATABASENAMEHERE;" +
                                                            "Uid=DATABASEUSERHERE;" +
                                                            "Pwd=DATABASEPASSWORDHERE;";

        public Form1()
        {
            InitializeComponent();

            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;

            backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            backgroundWorker2.ProgressChanged += backgroundWorker2_ProgressChanged;
            backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        //Login Button
        private void login_Click(object sender, EventArgs e)
        {
            if (username.Text.Length == 0)
            {
                MetroMessageBox.Show(this, "Please type in a Username", "Information", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (password.Text.Length == 0)
            {
                MetroMessageBox.Show(this, "Please type in a Password", "Information", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            metroLabel3.Text = "Waiting...";

            backgroundWorker1.RunWorkerAsync();
        }

        //Register Button
        private void register_Click(object sender, EventArgs e)
        {
            if (username.Text.Length == 0)
            {
                MetroMessageBox.Show(this, "Please type in a Username", "Information", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (password.Text.Length == 0)
            {
                MetroMessageBox.Show(this, "Please type in a Password", "Information", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            metroLabel3.Text = "Waiting...";

            backgroundWorker2.RunWorkerAsync();
        }

        #region Register Function

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Scramble Username

            var usernametext = EncryptionHelper.Encrypt(username.Text);

            var input1 = usernametext;
            var chars1 = input1.ToArray();
            var r1 = new Random(259);
            for (var i = 0; i < chars1.Length; i++)
            {
                var randomIndex = r1.Next(0, chars1.Length);
                var temp = chars1[randomIndex];
                chars1[randomIndex] = chars1[i];
                chars1[i] = temp;
            }

            //SCRAMBLED OUTPUT
            var scrambledusername = new string(chars1);

            #endregion

            #region Scramble Password

            var passwordtext = EncryptionHelper.Encrypt(password.Text);

            var input2 = passwordtext;
            var chars2 = input2.ToArray();
            var r2 = new Random(259);
            for (var i = 0; i < chars2.Length; i++)
            {
                var randomIndex = r2.Next(0, chars2.Length);
                var temp = chars2[randomIndex];
                chars2[randomIndex] = chars2[i];
                chars2[i] = temp;
            }

            //SCRAMBLED OUTPUT
            var scrambledpassword = new string(chars2);

            backgroundWorker2.ReportProgress(25);

            #endregion

            var HWID1 = EncryptionHelper.Encrypt(getUniqueID("C"));
            var HWID2 = EncryptionHelper.Encrypt(GetMachineGuid());


            var connection = new MySqlConnection(MyConnectionString);
            MySqlCommand cmd;
            try
            {
                backgroundWorker2.ReportProgress(43);

                connection.Open();

                try
                {
                    backgroundWorker2.ReportProgress(74);

                    var VerifyUser =
                        new MySqlCommand("SELECT count(*) from UserDB where (HWID1 = @HWID1) AND (HWID2 = @HWID2)",
                            connection);
                    VerifyUser.Parameters.AddWithValue("@HWID1", HWID1);
                    VerifyUser.Parameters.AddWithValue("@HWID2", HWID2);

                    var userCount = (int) (long) VerifyUser.ExecuteScalar();

                    if (userCount == 0)
                    {
                        cmd = connection.CreateCommand();
                        cmd.CommandText =
                            "INSERT INTO `UserDB` (`NAME`,`PASSWORD`,`HWID1`,`HWID2`,`PAID`) VALUES(@NAME, @PASSWORD, @HWID1, @HWID2, @PAID)";
                        cmd.Parameters.AddWithValue("@NAME", scrambledusername);
                        cmd.Parameters.AddWithValue("@PASSWORD", scrambledpassword);
                        cmd.Parameters.AddWithValue("@HWID1", HWID1);
                        cmd.Parameters.AddWithValue("@HWID2", HWID2);
                        cmd.Parameters.AddWithValue("@PAID", "0");
                        cmd.ExecuteNonQuery();

                        backgroundWorker2.ReportProgress(100);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            metroProgressSpinner1.Value = e.ProgressPercentage;

            if (metroProgressSpinner1.Value >= 1)
                metroLabel3.Text = "Working...";

            if (metroProgressSpinner1.Value >= 25)
                metroLabel3.Text = "Doing Secret Stuff...";

            if (metroProgressSpinner1.Value >= 43)
                metroLabel3.Text = "Connecting to Database...";

            if (metroProgressSpinner1.Value >= 74)
                metroLabel3.Text = "Creating Account...";
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                metroLabel3.Text = "Process was cancelled";
            else if (e.Error != null)
                metroLabel3.Text = "Error running the process.";
            else if (metroProgressSpinner1.Value == 25)
                metroLabel3.Text = "Doing Secret Stuff Failed!";
            else if (metroProgressSpinner1.Value == 43)
                metroLabel3.Text = "Cant Connect to Database!";
            else if (metroProgressSpinner1.Value == 74)
                metroLabel3.Text = "You already have an Account!";
            else if (metroProgressSpinner1.Value == 100)
                metroLabel3.Text = "User " + username.Text + " created.";
        }

        #endregion

        #region Login Function

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var connection = new MySqlConnection(MyConnectionString);
            try
            {
                backgroundWorker1.ReportProgress(10);

                #region Scramble Username

                var usernametext = EncryptionHelper.Encrypt(username.Text);

                var input1 = usernametext;
                var chars1 = input1.ToArray();
                var r1 = new Random(259);
                for (var i = 0; i < chars1.Length; i++)
                {
                    var randomIndex = r1.Next(0, chars1.Length);
                    var temp = chars1[randomIndex];
                    chars1[randomIndex] = chars1[i];
                    chars1[i] = temp;
                }

                //SCRAMBLED OUTPUT
                var scrambledusername = new string(chars1);

                #endregion

                #region Scramble Password

                var passwordtext = EncryptionHelper.Encrypt(password.Text);

                var input2 = passwordtext;
                var chars2 = input2.ToArray();
                var r2 = new Random(259);
                for (var i = 0; i < chars2.Length; i++)
                {
                    var randomIndex = r2.Next(0, chars2.Length);
                    var temp = chars2[randomIndex];
                    chars2[randomIndex] = chars2[i];
                    chars2[i] = temp;
                }

                //SCRAMBLED OUTPUT
                var scrambledpassword = new string(chars2);

                #endregion

                var HWID1 = EncryptionHelper.Encrypt(getUniqueID("C"));
                var HWID2 = EncryptionHelper.Encrypt(GetMachineGuid());

                backgroundWorker1.ReportProgress(25);

                try
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var VerifyUser =
                        new MySqlCommand(
                            "SELECT count(*) from UserDB where (NAME = @USERNAME) AND (PASSWORD = @PASSWORD)",
                            connection);
                    VerifyUser.Parameters.AddWithValue("@USERNAME", scrambledusername);
                    VerifyUser.Parameters.AddWithValue("@PASSWORD", scrambledpassword);

                    var userCount = (int) (long) VerifyUser.ExecuteScalar();

                    if (userCount >= 1)
                    {
                        backgroundWorker1.ReportProgress(45);

                        try
                        {
                            var VerifyUserHWID =
                                new MySqlCommand(
                                    "SELECT count(*) from UserDB where (HWID1 = @HWID1) AND (HWID2 = @HWID2)",
                                    connection);
                            VerifyUserHWID.Parameters.AddWithValue("@HWID1", HWID1);
                            VerifyUserHWID.Parameters.AddWithValue("@HWID2", HWID2);

                            var userCountHWID = (int) (long) VerifyUserHWID.ExecuteScalar();

                            if (userCountHWID >= 1)
                            {
                                backgroundWorker1.ReportProgress(85);

                                try
                                {
                                    var VerifyUserPaid1 =
                                        new MySqlCommand(
                                            "SELECT count(*) from UserDB where (NAME = @USERNAME) AND (PASSWORD = @PASSWORD) AND (HWID1 = @HWID1) AND (HWID2 = @HWID2) AND (PAID = @PAID)",
                                            connection);
                                    VerifyUserPaid1.Parameters.AddWithValue("@USERNAME", scrambledusername);
                                    VerifyUserPaid1.Parameters.AddWithValue("@PASSWORD", scrambledpassword);
                                    VerifyUserPaid1.Parameters.AddWithValue("@HWID1", HWID1);
                                    VerifyUserPaid1.Parameters.AddWithValue("@HWID2", HWID2);
                                    VerifyUserPaid1.Parameters.AddWithValue("@PAID", "1");

                                    var userCountPaid1 = (int) (long) VerifyUserPaid1.ExecuteScalar();

                                    if (userCountPaid1 >= 1)
                                        backgroundWorker1.ReportProgress(100);
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                                finally
                                {
                                    if (connection.State == ConnectionState.Open)
                                        connection.Close();
                                }
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
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

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            metroProgressSpinner1.Value = e.ProgressPercentage;

            if (metroProgressSpinner1.Value >= 1)
                metroLabel3.Text = "Connecting...";

            if (metroProgressSpinner1.Value >= 25)
                metroLabel3.Text = "Checking Login Details...";

            if (metroProgressSpinner1.Value >= 45)
                metroLabel3.Text = "Checking HWID...";

            if (metroProgressSpinner1.Value >= 85)
                metroLabel3.Text = "Checking Subscription Status...";
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                metroLabel3.Text = "Process was cancelled.";
            }
            else if (e.Error != null)
            {
                metroLabel3.Text = "Timeout while Checking Details.";
            }
            else if (metroProgressSpinner1.Value == 25)
            {
                metroLabel3.Text = "User not Found!";
            }
            else if (metroProgressSpinner1.Value == 45)
            {
                metroLabel3.Text = "HWID Changed!";
            }
            else if (metroProgressSpinner1.Value == 85)
            {
                metroLabel3.Text = "No Subscription!";
            }
            else if (metroProgressSpinner1.Value == 100)
            {
                metroLabel3.Text = "Login Complete";

                Hide();

                var F3 = new Form2();
                F3.Closed += (s, args) => Close();
                F3.Show();
            }
        }

        #endregion
    }
}