using System.Windows;
using System.Windows.Controls;

namespace ZipVersionControl
{
    /// <summary>
    /// Interaction logic for CommitMessageWindow.xaml
    /// </summary>
    public partial class CommitMessageWindow : Window
    {
        public string Message { get; set; }

        public CommitMessageWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you don't want to commit changes?\r\nANY UNCOMMITTED CHANGES WILL BE OVERWRITTEN!", "Don't commit changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Message = "";
                Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not Implemented");
        }

        private void txtCommitMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtCommitMessage.Text == "")
            {
                btnCommit.IsEnabled = false;
            }
            else
            {
                btnCommit.IsEnabled = true;
            }
        }

        private void btnCommit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Message = txtCommitMessage.Text;
            Close();
        }
    }
}
