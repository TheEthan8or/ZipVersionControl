using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Xml;

namespace ZipVersionControl
{
    class Session
    {
        public static int SelectedProfileIndex { get; set; } = -1;

        public static void Load()
        {

            // Save file path to variable
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZipVersionControl\\session.xml");
            // Check if AppData directory exists
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZipVersionControl")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZipVersionControl"));
            }
            // Load xml session file
            if (File.Exists(filePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                // Load previous session info
                SelectedProfileIndex = Convert.ToInt32(doc.DocumentElement.SelectSingleNode("SelectedProfileIndex").InnerText);
            }
        }

        public static void Save()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZipVersionControl\\session.xml");
            // Save profiles to file
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null); // Create xml declaration
            doc.AppendChild(docNode);
            XmlNode zipVersionControlNode = doc.CreateElement("ZipVersionControl"); // Create zip version control node
            doc.AppendChild(zipVersionControlNode);
            XmlNode selectedProfileIndexNode = doc.CreateElement("SelectedProfileIndex"); // Create selected profile index node
            zipVersionControlNode.AppendChild(selectedProfileIndexNode);
            selectedProfileIndexNode.InnerText = SelectedProfileIndex.ToString();
            doc.Save(filePath);
        }

        private static List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            foreach (string f in Directory.GetFiles(sDir))
            {
                files.Add(f);
            }
            foreach (string d in Directory.GetDirectories(sDir))
            {
                var temp = Path.GetFileName(d);
                if (sDir != Preferences.Profiles[SelectedProfileIndex].RepositoryPath || (sDir == Preferences.Profiles[SelectedProfileIndex].RepositoryPath && Path.GetFileName(d) != ".git"))
                {
                    files.AddRange(DirSearch(d));
                }
            }

            return files;
        }

        private static Signature GetSignature(DateTime signatureDate)
        {
            // Try to get name and email
            try
            {
                return new Signature(Preferences.GitName, Preferences.GitEmail, signatureDate);
            }
            catch (IndexOutOfRangeException)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OptionsWindow optionsWindow = new OptionsWindow();
                    // Show user input dialog to set name and email
                    MessageBox.Show("Please enter a name and email in the next dialog.", "Name and email required", MessageBoxButton.OK, MessageBoxImage.Information);
                    optionsWindow.ShowDialog();
                });
                return GetSignature(signatureDate);
            }
            catch (ArgumentException)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OptionsWindow optionsWindow = new OptionsWindow();
                    // Show user input dialog to set name and email
                    MessageBox.Show("Please enter a name and email in the next dialog.", "Name and email required", MessageBoxButton.OK, MessageBoxImage.Information);
                    optionsWindow.ShowDialog();
                });
                return GetSignature(signatureDate);
            }
        }

        public static void PullCurrentProfile(bool firstAttempt)
        {
            Repository repo = new Repository(Preferences.Profiles[SelectedProfileIndex].RepositoryPath);
            if (firstAttempt)
            {
                // Backup zip file and delete unnecessary backups
                Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath) + ".bks"));
                CopyZipFile(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath) + ".bks", DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss") + Path.GetExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)));
                while (Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)) + ".bks").Length > 6)
                {
                    string[] files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)) + ".bks");
                    Array.Sort(files);
                    File.Delete(files[0]);
                }
            }
            // Try to pull repository
            try
            {
                Preferences.Profiles[SelectedProfileIndex].Pull(new UsernamePasswordCredentials() { Username = Preferences.Profiles[SelectedProfileIndex].Username, Password = Encoding.Unicode.GetString(ProtectedData.Unprotect(Preferences.Profiles[SelectedProfileIndex].Password, null, DataProtectionScope.CurrentUser)) }, GetSignature(DateTime.Now));
                // Update latest commit time and zip file hash values in Profile
                if (repo.Head.Tip != null)
                {
                    Preferences.Profiles[SelectedProfileIndex].LatestCommitTime = repo.Head.Tip.Committer.When;
                }
                else
                {
                    Preferences.Profiles[SelectedProfileIndex].LatestCommitTime = DateTime.MinValue;
                }
                Preferences.Profiles[SelectedProfileIndex].ZipFileHash = HashZipFile();
                Preferences.Save();
            }
            catch (LibGit2SharpException ex)
            {
                if (ex.Message == "too many redirects or authentication replays" || ex.Message == "failed to set credentials: The parameter is incorrect.\r\n")
                {
                    if (firstAttempt)
                    {
                        Application.Current.Dispatcher.Invoke(() => { MessageBox.Show("Please type in your git credentials.", "Git credentials", MessageBoxButton.OK, MessageBoxImage.Information); });
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() => MessageBox.Show("Incorrect username or password. Please try again.", "Username or password incorrect", MessageBoxButton.OK, MessageBoxImage.Information));
                    }
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        GitCredentialsWindow gitCredentialsWindow = new GitCredentialsWindow();
                        gitCredentialsWindow.ShowDialog();
                    });
                }
                else if (ex.Message == "There is no tracking information for the current branch.")
                {
                    throw;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() => MessageBox.Show(ex.Message, "Pull error", MessageBoxButton.OK, MessageBoxImage.Error));
                }
                PullCurrentProfile(false);
            }
            catch (IOException)
            {
                Application.Current.Dispatcher.Invoke(() => MessageBox.Show("Zip file could not be accessed. Please make sure no other processes are using the file and that you have read/write access to it.", "Zip file could not be read/written", MessageBoxButton.OK, MessageBoxImage.Error));
                PullCurrentProfile(false);
            }
        }

        private static void CopyZipFile(string destination)
        {
            try
            {
                File.Copy(Preferences.Profiles[SelectedProfileIndex].ZipFilePath, destination);
            }
            catch (IOException)
            {
                Application.Current.Dispatcher.Invoke(() => MessageBox.Show("Zip file could not be accessed. Please make sure no other processes are using the file and that you have read/write access to it.", "Zip file could not be read/written", MessageBoxButton.OK, MessageBoxImage.Error));
                CopyZipFile(destination);
            }
        }

        public static void PushCurrentProfile(bool firstAttempt)
        {
            Repository repo = new Repository(Preferences.Profiles[SelectedProfileIndex].RepositoryPath);
            // Try to pull repository
            try
            {
                Preferences.Profiles[SelectedProfileIndex].Push(new UsernamePasswordCredentials() { Username = Preferences.Profiles[SelectedProfileIndex].Username, Password = Encoding.Unicode.GetString(ProtectedData.Unprotect(Preferences.Profiles[SelectedProfileIndex].Password, null, DataProtectionScope.CurrentUser)) });
            }
            catch (LibGit2SharpException ex)
            {
                if (ex.Message == "too many redirects or authentication replays")
                {
                    if (firstAttempt)
                    {
                        Application.Current.Dispatcher.Invoke(() => { MessageBox.Show("Please type in your git credentials.", "Git credentials", MessageBoxButton.OK, MessageBoxImage.Warning); });
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() => MessageBox.Show("Incorrect username or password. Please try again.", "Username or password incorrect", MessageBoxButton.OK, MessageBoxImage.Information));
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GitCredentialsWindow gitCredentialsWindow = new GitCredentialsWindow();
                        gitCredentialsWindow.ShowDialog();
                    });
                }
                else if (ex.Message == "cannot push non-fastforwardable reference")
                {
                    Application.Current.Dispatcher.Invoke(() => MessageBox.Show("The changes pulled from the remote have conflicted with the local changes.\r\nResolve these confilcts. Then, proceed by clicking 'OK'.", "Merge conflicts", MessageBoxButton.OK, MessageBoxImage.Information));
                    if (repo.RetrieveStatus().IsDirty)
                    {
                        // Commit changes
                        string commitMessage = "";
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            CommitMessageWindow commitMessageWindow = new CommitMessageWindow();
                            commitMessageWindow.btnNoCommit.IsEnabled = false;
                            commitMessageWindow.txtCommitMessage.Text = "Merge branch '" + repo.Head.FriendlyName + "' of " + repo.Network.Remotes.First().Url; ;
                            commitMessageWindow.ShowDialog();
                            commitMessage = commitMessageWindow.Message;
                        });
                        Commands.Stage(repo, "*");
                        DateTime signatureDate = DateTime.Now;
                        Commit commit = repo.Commit(commitMessage, GetSignature(signatureDate), GetSignature(signatureDate));
                    }
                    // Backup zip file and delete unnecessary backups
                    Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath) + ".bks"));
                    File.Copy(Preferences.Profiles[SelectedProfileIndex].ZipFilePath, Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath) + ".bks", DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss") + Path.GetExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)));
                    while (Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)) + ".bks").Length > 6)
                    {
                        string[] files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)) + ".bks");
                        Array.Sort(files);
                        File.Delete(files[0]);
                    }
                    // Replace zip file
                    DeleteZipFile();
                    using (ZipArchive zipArchive = ZipFile.Open(Preferences.Profiles[SelectedProfileIndex].ZipFilePath, ZipArchiveMode.Create))
                    {
                        foreach (string f in DirSearch(Preferences.Profiles[SelectedProfileIndex].RepositoryPath))
                        {
                            zipArchive.CreateEntryFromFile(f, f.Replace(Preferences.Profiles[SelectedProfileIndex].RepositoryPath + @"\", ""));
                        }
                    }
                    // Update latest commit time and zip file hash values in Profile
                    if (repo.Head.Tip != null)
                    {
                        Preferences.Profiles[SelectedProfileIndex].LatestCommitTime = repo.Head.Tip.Committer.When;
                    }
                    else
                    {
                        Preferences.Profiles[SelectedProfileIndex].LatestCommitTime = DateTime.MinValue;
                    }
                    Preferences.Profiles[SelectedProfileIndex].ZipFileHash = HashZipFile();
                    Preferences.Save();
                }
                else if (ex.Message == "There is no tracking information for the current branch.")
                {
                    throw;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() => MessageBox.Show(ex.Message, "Push error", MessageBoxButton.OK, MessageBoxImage.Information));
                }
                PushCurrentProfile(false);
            }
        }

        public static void CommitCurrentProfile()
        {
            // Commit changes
            Repository repo = new Repository(Preferences.Profiles[SelectedProfileIndex].RepositoryPath);
            bool repositoryChange = false;
            bool zipFileChange = false;
            byte[] repositoryHash;
            byte[] zipFileHash;
            // Check if there are uncommitted files in the repository
            if (repo.RetrieveStatus().IsDirty)
            {
                MessageBoxResult messageBoxResult = new MessageBoxResult();
                Application.Current.Dispatcher.Invoke(() => { messageBoxResult = MessageBox.Show("There are uncommitted changes in the repository. Would you like to include them in the commit? If you don't, they will be overwritten.\r\n(Note: Unresolved merge conflicts may be included in the commit, even if you click 'No'.", "Uncommitted changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question); });
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    repositoryChange = true;
                }
                else if (messageBoxResult == MessageBoxResult.No)
                {
                    repo.Reset(ResetMode.Mixed, repo.Head.Tip);
                }
                else if (messageBoxResult == MessageBoxResult.Cancel) { throw new NotImplementedException(); }
            }
            if (File.Exists(Preferences.Profiles[SelectedProfileIndex].ZipFilePath))
            {
                // Check for differences between contents in repository and contents in zip file
                // Delete Temp directory if exists
                if (Directory.Exists(Path.Combine(Path.GetTempPath(), "ZipVersionControl")))
                {
                    Directory.Delete(Path.Combine(Path.GetTempPath(), "ZipVersionControl"), true);
                }
                // Create Temp directory
                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "ZipVersionControl"));
                // Create temporary zip file of repository to hash
                using (ZipArchive zipArchive = ZipFile.Open(Path.Combine(Path.GetTempPath(), "ZipVersionControl\\" + Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath) + ".rep"), ZipArchiveMode.Create))
                {
                    foreach (string f in DirSearch(Preferences.Profiles[SelectedProfileIndex].RepositoryPath))
                    {
                        zipArchive.CreateEntryFromFile(f, f.Replace(Preferences.Profiles[SelectedProfileIndex].RepositoryPath + @"\", ""));
                    }
                }
                // Re-zip zip file for correct hashing
                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "ZipVersionControl\\" + Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)));
                UnzipZipFile(Path.Combine(Path.GetTempPath(), "ZipVersionControl\\" + Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)));
                using (ZipArchive zipArchive = ZipFile.Open(Path.Combine(Path.GetTempPath(), "ZipVersionControl\\" + Path.GetFileName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)), ZipArchiveMode.Create))
                {
                    foreach (string f in DirSearch(Preferences.Profiles[SelectedProfileIndex].RepositoryPath))
                    {
                        zipArchive.CreateEntryFromFile(f, f.Replace(Preferences.Profiles[SelectedProfileIndex].RepositoryPath + @"\", "")); // Compress each file to zip file in Temp directory
                    }
                }
                // Hash files
                using (FileStream stream = File.OpenRead(Path.Combine(Path.GetTempPath(), "ZipVersionControl\\" + Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath) + ".rep")))
                {
                    repositoryHash = MD5.Create().ComputeHash(stream);
                    stream.Close();
                }
                using (FileStream stream = File.OpenRead(Path.Combine(Path.GetTempPath(), "ZipVersionControl\\" + Path.GetFileName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath))))
                {
                    zipFileHash = MD5.Create().ComputeHash(stream);
                    stream.Close();
                }
                // Delete temporary files
                Directory.Delete(Path.Combine(Path.GetTempPath(), "ZipVersionControl"), true);
                // Compare hashes
                if (!repositoryHash.SequenceEqual(zipFileHash))
                {
                    // Check for modifications to repository
                    Commit test = repo.Head.Tip;
                    if (repo.Head.Tip != null && (Preferences.Profiles[SelectedProfileIndex].LatestCommitTime.Equals(DateTime.MinValue) || Preferences.Profiles[SelectedProfileIndex].LatestCommitTime < repo.Head.Tip.Committer.When))
                    {
                        repositoryChange = true;
                    }

                    // Check for modifications to zip file
                    if (!zipFileHash.SequenceEqual(Preferences.Profiles[SelectedProfileIndex].ZipFileHash))
                    {
                        zipFileChange = true;
                    }
                }
                else
                {
                    repositoryChange = false;
                    zipFileChange = false;
                }

                // Run action depending on what has been changed
                if (repositoryChange && zipFileChange)
                {
                    bool keepRepositoryFiles = true;
                    bool keepZipFileFiles = true;
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        CommitConflictResolutionWindow commitConflictResolutionWindow = new CommitConflictResolutionWindow();
                        if (repo.RetrieveStatus().IsDirty)
                        {
                            // Disable "Keep files in zip file" button
                            commitConflictResolutionWindow.btnZipFileFiles.IsEnabled = false;
                        }
                        commitConflictResolutionWindow.ShowDialog();
                        keepRepositoryFiles = commitConflictResolutionWindow.KeepRepositoryFiles;
                        keepZipFileFiles = commitConflictResolutionWindow.KeepZipFileFiles;
                    });
                    if (keepRepositoryFiles && keepZipFileFiles)
                    {
                        // Merge files
                        throw new NotImplementedException();
                        //Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory
                    }
                    else if (keepRepositoryFiles && !keepZipFileFiles)
                    {
                        // Commit changes if there are uncommitted changes in the repository
                        if (repo.RetrieveStatus().IsDirty)
                        {
                            string commitMessage = "";
                            Application.Current.Dispatcher.Invoke(() => 
                            {
                                CommitMessageWindow commitMessageWindow = new CommitMessageWindow();
                                commitMessageWindow.btnNoCommit.IsEnabled = false;
                                commitMessageWindow.ShowDialog();
                                commitMessage = commitMessageWindow.Message;
                            });
                            Commands.Stage(repo, "*");
                            DateTime signatureDate = DateTime.Now;
                            Commit commit = repo.Commit(commitMessage, GetSignature(signatureDate), GetSignature(signatureDate));
                        }
                        // Backup zip file and delete unnecessary backups
                        Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath) + ".bks"));
                        CopyZipFile(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath) + ".bks", DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss") + Path.GetExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)));
                        while (Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)) + ".bks").Length > 6)
                        {
                            string[] files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)) + ".bks");
                            Array.Sort(files);
                            File.Delete(files[0]);
                        }
                        // Replace zip file
                        DeleteZipFile();
                        using (ZipArchive zipArchive = ZipFile.Open(Preferences.Profiles[SelectedProfileIndex].ZipFilePath, ZipArchiveMode.Create))
                        {
                            foreach (string f in DirSearch(Preferences.Profiles[SelectedProfileIndex].RepositoryPath))
                            {
                                zipArchive.CreateEntryFromFile(f, f.Replace(Preferences.Profiles[SelectedProfileIndex].RepositoryPath + @"\", ""));
                            }
                        }
                    }
                    else if (!keepRepositoryFiles && keepZipFileFiles)
                    {
                        // Commit changes in zip file and overwrite repository files
                        string commitMessage = "";
                        Application.Current.Dispatcher.Invoke(() => 
                        {
                            CommitMessageWindow commitMessageWindow = new CommitMessageWindow();
                            commitMessageWindow.btnNoCommit.IsEnabled = false;
                            commitMessageWindow.ShowDialog();
                            commitMessage = commitMessageWindow.Message;
                        });
                        Preferences.Profiles[SelectedProfileIndex].Commit(commitMessage, GetSignature(DateTime.Now));
                    }
                }
                else if (repositoryChange && !zipFileChange && repo.RetrieveStatus().IsDirty)
                {
                    // Commit changes
                    string commitMessage = "";
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        CommitMessageWindow commitMessageWindow = new CommitMessageWindow();
                        commitMessageWindow.btnNoCommit.IsEnabled = false;
                        commitMessageWindow.ShowDialog();
                        commitMessage = commitMessageWindow.Message;
                    });
                    Commands.Stage(repo, "*");
                    DateTime signatureDate = DateTime.Now;
                    Commit commit = repo.Commit(commitMessage, GetSignature(signatureDate), GetSignature(signatureDate));
                    // Backup zip file and delete unnecessary backups
                    Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath) + ".bks"));
                    File.Copy(Preferences.Profiles[SelectedProfileIndex].ZipFilePath, Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath) + ".bks", DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss") + Path.GetExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)));
                    while (Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)) + ".bks").Length > 6)
                    {
                        string[] files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)) + ".bks");
                        Array.Sort(files);
                        File.Delete(files[0]);
                    }
                    // Replace zip file
                    DeleteZipFile();
                    using (ZipArchive zipArchive = ZipFile.Open(Preferences.Profiles[SelectedProfileIndex].ZipFilePath, ZipArchiveMode.Create))
                    {
                        foreach (string f in DirSearch(Preferences.Profiles[SelectedProfileIndex].RepositoryPath))
                        {
                            zipArchive.CreateEntryFromFile(f, f.Replace(Preferences.Profiles[SelectedProfileIndex].RepositoryPath + @"\", ""));
                        }
                    }
                }
                else if (!repositoryChange && zipFileChange)
                {
                    // Commit changes in zip file
                    string commitMessage = "";
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        CommitMessageWindow commitMessageWindow = new CommitMessageWindow();
                        commitMessageWindow.ShowDialog();
                        commitMessage = commitMessageWindow.Message;
                    });
                    if (commitMessage == "")
                    {
                        // Backup zip file and delete unnecessary backups
                        Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath) + ".bks"));
                        File.Copy(Preferences.Profiles[SelectedProfileIndex].ZipFilePath, Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath) + ".bks", DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss") + Path.GetExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)));
                        while (Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)) + ".bks").Length > 6)
                        {
                            string[] files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Preferences.Profiles[SelectedProfileIndex].ZipFilePath), Path.GetFileNameWithoutExtension(Preferences.Profiles[SelectedProfileIndex].ZipFilePath)) + ".bks");
                            Array.Sort(files);
                            File.Delete(files[0]);
                        }
                        // Replace zip file
                        DeleteZipFile();
                        using (ZipArchive zipArchive = ZipFile.Open(Preferences.Profiles[SelectedProfileIndex].ZipFilePath, ZipArchiveMode.Create))
                        {
                            foreach (string f in DirSearch(Preferences.Profiles[SelectedProfileIndex].RepositoryPath))
                            {
                                zipArchive.CreateEntryFromFile(f, f.Replace(Preferences.Profiles[SelectedProfileIndex].RepositoryPath + @"\", ""));
                            }
                        }
                    }
                    else
                    {
                        Preferences.Profiles[SelectedProfileIndex].Commit(commitMessage, GetSignature(DateTime.Now));
                    }
                }
                else if (!repositoryChange && !zipFileChange && repo.RetrieveStatus().IsDirty)
                {
                    // Commit changes
                    string commitMessage = "";
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CommitMessageWindow commitMessageWindow = new CommitMessageWindow();
                        commitMessageWindow.btnNoCommit.IsEnabled = false;
                        commitMessageWindow.ShowDialog();
                        commitMessage = commitMessageWindow.Message;
                    });
                    Commands.Stage(repo, "*");
                    DateTime signatureDate = DateTime.Now;
                    Commit commit = repo.Commit(commitMessage, GetSignature(signatureDate), GetSignature(signatureDate));
                }
            }
            else
            {
                // Commit any uncommitted changes in repository
                if (repo.RetrieveStatus().IsDirty)
                {
                    string commitMessage = "";
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CommitMessageWindow commitMessageWindow = new CommitMessageWindow();
                        commitMessageWindow.btnNoCommit.IsEnabled = false;
                        commitMessageWindow.ShowDialog();
                        commitMessage = commitMessageWindow.Message;
                    });
                    Commands.Stage(repo, "*");
                    DateTime signatureDate = DateTime.Now;
                    Commit commit = repo.Commit(commitMessage, GetSignature(signatureDate), GetSignature(signatureDate));
                }
                // Create zip file
                using (ZipArchive zipArchive = ZipFile.Open(Preferences.Profiles[SelectedProfileIndex].ZipFilePath, ZipArchiveMode.Create))
                {
                    foreach (string f in DirSearch(Preferences.Profiles[SelectedProfileIndex].RepositoryPath))
                    {
                        zipArchive.CreateEntryFromFile(f, f.Replace(Preferences.Profiles[SelectedProfileIndex].RepositoryPath + @"\", ""));
                    }
                }
                // Hash zip file
                zipFileHash = HashZipFile();
            }
            // Update latest commit time and zip file hash values in Profile
            if (repo.Head.Tip != null)
            {
                Preferences.Profiles[SelectedProfileIndex].LatestCommitTime = repo.Head.Tip.Committer.When;
            }
            else
            {
                Preferences.Profiles[SelectedProfileIndex].LatestCommitTime = DateTime.MinValue;
            }
            Preferences.Profiles[SelectedProfileIndex].ZipFileHash = zipFileHash;
            Preferences.Save();
        }

        private static byte[] HashZipFile()
        {
            try
            {
                using (FileStream stream = File.OpenRead(Preferences.Profiles[SelectedProfileIndex].ZipFilePath))
                {
                    return MD5.Create().ComputeHash(stream);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Zip file could not be accessed. Please make sure no other processes are using the file and that you have read/write access to it.", "Zip file could not be read/written", MessageBoxButton.OK, MessageBoxImage.Error);
                return HashZipFile();
            }
        }

        private static void DeleteZipFile()
        {
            try
            {
                File.Delete(Preferences.Profiles[SelectedProfileIndex].ZipFilePath);
            }
            catch (IOException)
            {
                MessageBox.Show("Zip file could not be accessed. Please make sure no other processes are using the file and that you have read/write access to it.", "Zip file could not be read/written", MessageBoxButton.OK, MessageBoxImage.Error);
                DeleteZipFile();
            }
        }

        private static void UnzipZipFile(string directory)
        {
            try
            {
                using (FileStream zipFile = new FileStream(Preferences.Profiles[SelectedProfileIndex].ZipFilePath, FileMode.Open))
                {
                    new ZipArchive(zipFile, ZipArchiveMode.Update).ExtractToDirectory(directory);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Zip file could not be accessed. Please make sure no other processes are using the file and that you have read/write access to it.", "Zip file could not be read/written", MessageBoxButton.OK, MessageBoxImage.Error);
                UnzipZipFile(directory);
            }
        }
    }
}
