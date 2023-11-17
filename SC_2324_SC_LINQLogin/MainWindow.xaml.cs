using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SC_2324_SC_LINQLogin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SampleLINQDataContext db = new SampleLINQDataContext();

        public MainWindow()
        {
            InitializeComponent();

            db = new SampleLINQDataContext
                (Properties.Settings.Default.SpecialClassLoginThingConnectionString);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(txtb_User.Text.Length > 0) 
            {
                if(txtb_Pass.Text.Length > 0) 
                {
                    // proceed to login
                    //MessageBox.Show($"Username {txtb_User.Text} Password {txtb_Pass.Text}");
                    if(passComparison(getPassword()) == 0)
                    {
                        MessageBox.Show("Login Success", "Welcome Back"
                            , MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        insertLog(txtb_User.Text, "Login Success");
                        updateLastLogin(txtb_User.Text);
                    }
                    else if(passComparison(getPassword()) == -1)
                    {
                        MessageBox.Show("Invalid Username and/or Password", "Login Failed"
                            , MessageBoxButton.OK, MessageBoxImage.Warning);
                        insertLog(null, "Brute Force Attempt");
                    }
                    else
                    {
                        MessageBox.Show("Invalid Username and/or Password", "Login Failed"
                            , MessageBoxButton.OK, MessageBoxImage.Warning);
                        insertLog(txtb_User.Text, "Incorrect Password");
                    }
                }
                else
                {
                    // inform user to input password
                    MessageBox.Show("Please input a valid password.","Blank Password"
                        , MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private string getPassword()
        {
            string uPass = "";
            // var users = from s in db_con.tblUsers where s.UserID == WindowManager.getUID() select s;
            var users = from s in db.Table_Users where s.UserID == txtb_User.Text select s;
            /*
             * select
             *        *
             * from Table_User
             * where
             *        UserID = txtb_User.Text
             */

            //MessageBox.Show(users.Count().ToString());

            if(users.Count() == 1)
            {
                foreach(Table_User user in users) 
                {
                    uPass = user.UserPass;
                }
            }
            else
            {
                uPass = null;
            }

            return uPass;
        }

        private int passComparison(string uPass)
        {
            if (txtb_Pass.Text == uPass)
                return 0;
            else if (uPass == null)
                return -1;
            else
                return 1;
        }

        private void insertLog(string user, string message)
        {
            Table_Log log = new Table_Log
            {
                    UserID = user
                ,   LogMessage = message
                ,   LogDate = DateTime.Now
            };

            db.Table_Logs.InsertOnSubmit(log);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error when inserting: {e.Message}");
            }
        }

        private void updateLastLogin(string user)
        {
            DateTime lastLogin = DateTime.Now;

            var users = from s in db.Table_Users where s.UserID == user select s;

            foreach(Table_User u in users) 
            {
                u.LastLoginDate = lastLogin;
            }

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error when inserting: {e.Message}");
            }

        }
    }
}
