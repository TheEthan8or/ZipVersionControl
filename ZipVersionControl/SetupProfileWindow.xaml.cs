using LibGit2Sharp;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using ZipVersionControlLib;

namespace ZipVersionControl
{
    /// <summary>
    /// Interaction logic for SetupProfileWindow.xaml
    /// </summary>
    public partial class SetupProfileWindow : Window
    {
        public SetupProfileWindow()
        {
            InitializeComponent();
        }

        public void SetupLayoutForAddingProfile()
        {
            wdwSetupProfile.Title = "Add profile";
            btnRemoveProfile.Visibility = Visibility.Hidden;
            CompleteInformationCheck();
        }

        public void SetupLayoutForEditingProfile()
        {
            wdwSetupProfile.Title = "Profile settings";
            txtProfileName.Text = Preferences.Profiles[Session.SelectedProfileIndex].ProfileName;
            txtZipFile.Text = Preferences.Profiles[Session.SelectedProfileIndex].ZipFilePath;
            txtGitRepository.Text = Preferences.Profiles[Session.SelectedProfileIndex].RepositoryPath;
            Preferences.Profiles.RemoveAt(Session.SelectedProfileIndex);
            CompleteInformationCheck();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Select zip file
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            if (txtZipFile.Text != "")
            {
                dlg.InitialDirectory = System.IO.Path.GetDirectoryName(txtZipFile.Text);
                dlg.FileName = System.IO.Path.GetFileName(txtZipFile.Text);
            }
            dlg.DefaultExt = "zip";
            dlg.OverwritePrompt = false;
            dlg.Title = "Select Zip File";
            dlg.Filter = "All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                txtZipFile.Text = filename;
            }
            CompleteInformationCheck();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Select git repository
            using (var dialog = new FolderBrowserDialog())
            {
                if (txtGitRepository.Text != "")
                {
                    dialog.SelectedPath = txtGitRepository.Text;
                }
                DialogResult result = dialog.ShowDialog();
                if (dialog.SelectedPath != "")
                {
                    txtGitRepository.Text = dialog.SelectedPath;
                }
            }
            CompleteInformationCheck();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Save profile settings
            Preferences.Profiles.Add(new Profile() { ProfileName = txtProfileName.Text, ZipFilePath = txtZipFile.Text, RepositoryPath = txtGitRepository.Text, Username = "", Password = ProtectedData.Protect(Encoding.Unicode.GetBytes(""), null, DataProtectionScope.CurrentUser), ZipFileHash = Encoding.Unicode.GetBytes("") });
            Preferences.Profiles.Sort(delegate (Profile p1, Profile p2) { return p1.ProfileName.CompareTo(p2.ProfileName); });
            Preferences.Save();
            Session.SelectedProfileIndex = Preferences.Profiles.FindIndex(profile => profile.ProfileName.Equals(txtProfileName.Text, StringComparison.Ordinal));
            Session.Save();
            Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtProfileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CompleteInformationCheck();
        }

        private void CompleteInformationCheck()
        {
            btnDone.IsEnabled = true;
            txtProfileName.Background = Brushes.White;
            txtZipFile.Background = Brushes.White;
            txtGitRepository.Background = Brushes.White;
            if (txtProfileName.Text != "")
            {
                if (Preferences.Profiles.Find(profile => profile.ProfileName.Equals(txtProfileName.Text, StringComparison.Ordinal)) != null)
                {
                    btnDone.IsEnabled = false;
                    txtProfileName.Background = Brushes.Red;
                }
            }
            else
            {
                btnDone.IsEnabled = false;
            }
            if (txtZipFile.Text != "")
            {
                try
                {
                    ZipFile.OpenRead(txtZipFile.Text).Dispose();
                }
                catch (InvalidDataException)
                {
                    btnDone.IsEnabled = false;
                    txtZipFile.Background = Brushes.Red;
                }
                catch (IOException) { }
            }
            else
            {
                btnDone.IsEnabled = false;
            }
            if (txtGitRepository.Text != "")
            {
                if (Directory.Exists(Path.Combine(txtGitRepository.Text, ".git")))
                {
                    try
                    {
                        //new Repository(txtGitRepository.Text);
                    }
                    catch (RepositoryNotFoundException)
                    {
                        btnDone.IsEnabled = false;
                        txtGitRepository.Background = Brushes.Red;
                    }
                }
                else
                {
                    btnDone.IsEnabled = false;
                    txtGitRepository.Background = Brushes.Red;
                }
            }
            else
            {
                btnDone.IsEnabled = false;
            }
        }

        private void btnRemoveProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to remove this profile?", "Remove profile", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Preferences.Save();
                if (Session.SelectedProfileIndex == Preferences.Profiles.Count)
                {
                    Session.SelectedProfileIndex--;
                }
                Close();
            }
        }
    }
}
