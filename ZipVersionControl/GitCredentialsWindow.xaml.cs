using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace ZipVersionControl
{
    /// <summary>
    /// Interaction logic for GitCredentialsWindow.xaml
    /// </summary>
    public partial class GitCredentialsWindow : Window
    {
        public GitCredentialsWindow()
        {
            InitializeComponent();
            txtUsername.Text = Preferences.Profiles[Session.SelectedProfileIndex].Username;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Encrypt password
            byte[] plainPassword = Encoding.Unicode.GetBytes(pwdPassword.Password);
            pwdPassword.Password = "";
            Preferences.Profiles[Session.SelectedProfileIndex].Password = ProtectedData.Protect(plainPassword, null, DataProtectionScope.CurrentUser);
            Preferences.Profiles[Session.SelectedProfileIndex].Username = txtUsername.Text;
            Preferences.Save();
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not Implemented");
        }
    }
}
