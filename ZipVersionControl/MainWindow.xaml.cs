using System.Windows;
using System.Windows.Controls;

namespace ZipVersionControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Initialize preferences and load previous session
            Preferences.Load();
            Session.Load();
            cbxProfileSelector.SelectedIndex = Session.SelectedProfileIndex;
            cbxProfileSelector.ItemsSource = Preferences.Profiles;
            if (Preferences.Profiles.Count != 0)
            {
                btnSettings.IsEnabled = true;
                btnSync.IsEnabled = true;
                cbxSync.IsEnabled = true;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetupProfileWindow setupProfileWindow = new SetupProfileWindow();
            setupProfileWindow.SetupLayoutForAddingProfile();
            setupProfileWindow.ShowDialog();
            // Refresh Profile Selector
            cbxProfileSelector.ItemsSource = null;
            cbxProfileSelector.SelectedIndex = Session.SelectedProfileIndex;
            cbxProfileSelector.ItemsSource = Preferences.Profiles;
            if (Preferences.Profiles.Count != 0)
            {
                btnSettings.IsEnabled = true;
                btnSync.IsEnabled = true;
                cbxSync.IsEnabled = true;
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            OptionsWindow optionsWindow = new OptionsWindow();
            optionsWindow.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Session.SelectedProfileIndex = cbxProfileSelector.SelectedIndex;
            Session.Save();
            ProfileActionWindow profileActionWindow = new ProfileActionWindow();
            profileActionWindow.SyncProfile();
            profileActionWindow.ShowDialog();
        }

        private void cbxProfileSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxProfileSelector.ItemsSource == Preferences.Profiles)
            {
                Session.SelectedProfileIndex = cbxProfileSelector.SelectedIndex;
                Session.Save();
            }
        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            Session.SelectedProfileIndex = cbxProfileSelector.SelectedIndex;
            Session.Save();
            ProfileActionWindow profileActionWindow = new ProfileActionWindow();
            profileActionWindow.CommitProfile();
            profileActionWindow.ShowDialog();
        }

        private void cbxSync_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbxSync.SelectedIndex = -1;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            Session.SelectedProfileIndex = cbxProfileSelector.SelectedIndex;
            Session.Save();
            SetupProfileWindow setupProfileWindow = new SetupProfileWindow();
            setupProfileWindow.SetupLayoutForEditingProfile();
            setupProfileWindow.ShowDialog();
            // Refresh Profile Selector
            Preferences.Load();
            cbxProfileSelector.ItemsSource = null;
            cbxProfileSelector.SelectedIndex = Session.SelectedProfileIndex;
            cbxProfileSelector.ItemsSource = Preferences.Profiles;
            if (Preferences.Profiles.Count == 0)
            {
                btnSettings.IsEnabled = false;
                btnSync.IsEnabled = false;
                cbxSync.IsEnabled = false;
            }
        }
    }
}
