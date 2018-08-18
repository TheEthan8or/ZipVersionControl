using LibGit2Sharp;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Windows;

namespace ZipVersionControl
{
    /// <summary>
    /// Interaction logic for ProfileActionWindow.xaml
    /// </summary>
    public partial class ProfileActionWindow : Window
    {
        BackgroundWorker bw = new BackgroundWorker();

        public ProfileActionWindow()
        {
            InitializeComponent();
        }

        public void SyncProfile()
        {
            wdwProfileAction.Title = "Syncing profile '" + Preferences.Profiles[Session.SelectedProfileIndex].ProfileName + "'...";
            bw.WorkerReportsProgress = true;
            bw.DoWork += bw_SyncProfile;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.RunWorkerAsync();
        }

        public void CommitProfile()
        {
            tblStatus.Text = "Committing profile '" + Preferences.Profiles[Session.SelectedProfileIndex].ProfileName + "'...";
            bw.DoWork += bw_CommitProfile;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.RunWorkerAsync();
        }

        private void bw_CommitProfile(object sender, DoWorkEventArgs e)
        {
            try
            {
                new Repository(Preferences.Profiles[Session.SelectedProfileIndex].RepositoryPath);
                if (File.Exists(Preferences.Profiles[Session.SelectedProfileIndex].ZipFilePath))
                {
                    ZipFile.OpenRead(Preferences.Profiles[Session.SelectedProfileIndex].ZipFilePath).Dispose();
                }
            }
            catch (RepositoryNotFoundException ex)
            {
                e.Result = ex;
                return;
            }
            catch (InvalidDataException ex)
            {
                e.Result = ex;
                return;
            }

            Session.CommitCurrentProfile();
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                MessageBox.Show("Operation successful!", "ZipVersionControl", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (e.Result is RepositoryNotFoundException)
            {
                MessageBox.Show("Repository path does not point to a valid repository. Your path may be incorrect, or the repository may be corrupt.", "Invalid repository", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e.Result is InvalidDataException)
            {
                MessageBox.Show("Zip file path does not point to a valid zip file. Your path may be incorrect, or the zip file may be corrupt.", "Invalid zip file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e.Result is LibGit2SharpException && (e.Result as Exception).Message is "There is no tracking information for the current branch.")
            {
                MessageBox.Show("Syncing the repository requires it to have a remote. Specify a remote and try again.", "No remotes specified", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Unknown error: " + (e.Result as Exception).Message, "ZipVersionControl", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            Close();
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string[] results = (string[])e.UserState;
            tblStatus.Text = results[e.ProgressPercentage];
            pgbProgress.IsIndeterminate = false;
            pgbProgress.Value = e.ProgressPercentage;
        }

        private void bw_SyncProfile(object sender, DoWorkEventArgs e)
        {
            string[] workerResult = new string[6];
            workerResult[0] = "Committing changes...";
            workerResult[1] = "Pulling changes to local repository and saving them to zip file...";
            workerResult[2] = "Pushing changes to remote repository...";
            workerResult[3] = "Operation successful!";

            try
            {
                new Repository(Preferences.Profiles[Session.SelectedProfileIndex].RepositoryPath);
                if (File.Exists(Preferences.Profiles[Session.SelectedProfileIndex].ZipFilePath))
                {
                    ZipFile.OpenRead(Preferences.Profiles[Session.SelectedProfileIndex].ZipFilePath).Dispose();
                }
            }
            catch (RepositoryNotFoundException ex)
            {
                e.Result = ex;
                return;
            }
            catch (InvalidDataException ex)
            {
                e.Result = ex;
                return;
            }
            catch (Exception ex)
            {
                e.Result = ex;
                return;
            }

            // Commit changes and make contents of zip file same as contents of repository
            bw.ReportProgress(0, workerResult);
            Session.CommitCurrentProfile();

            // Pull git repository and save changes to zip file
            bw.ReportProgress(1, workerResult);
            try
            {
                Session.PullCurrentProfile(true);
            }
            catch (MergeFetchHeadNotFoundException ex)
            {
                e.Result = ex;
                return;
            }
            catch (LibGit2SharpException ex)
            {
                e.Result = ex;
                return;
            }

            // Push repository
            bw.ReportProgress(2, workerResult);
            Session.PushCurrentProfile(true);

            // Report success
            bw.ReportProgress(3, workerResult);
        }
    }
}
