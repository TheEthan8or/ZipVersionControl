using System.Windows;

namespace ZipVersionControl
{
    /// <summary>
    /// Interaction logic for CommitConflictResolutionWindow.xaml
    /// </summary>
    public partial class CommitConflictResolutionWindow : Window
    {
        public bool KeepRepositoryFiles = false;

        public bool KeepZipFileFiles = false;

        public CommitConflictResolutionWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not Implemented");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            KeepRepositoryFiles = true;
            Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            KeepZipFileFiles = true;
            Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not Implemented");
            return;
        }
    }
}
