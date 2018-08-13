using System.Windows;

namespace ZipVersionControl
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();
            txtName.Text = Preferences.GitName;
            txtEmail.Text = Preferences.GitEmail;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Preferences.GitName = txtName.Text;
            Preferences.GitEmail = txtEmail.Text;
            Preferences.Save();
            Close();
        }
    }
}
