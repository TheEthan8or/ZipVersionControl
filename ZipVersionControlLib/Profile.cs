using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ZipVersionControlLib
{
    public class Profile
    {
        public string ProfileName { get; set; }
        public string ZipFilePath { get; set; }
        public string RepositoryPath { get; set; }
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public DateTimeOffset LatestCommitTime { get; set; }
        public byte[] ZipFileHash { get; set; }

        public void Commit(string commitMessage, Signature signature)
        {
            Repository repo = new Repository(RepositoryPath);
            // Replace old files in local git repository with new files
            DirectoryInfo di = new DirectoryInfo(RepositoryPath);
            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                if (dir.Name != ".git")
                {
                    dir.Delete(true);
                }
            }
            using (FileStream zipFile = new FileStream(ZipFilePath, FileMode.Open))
            {
                new ZipArchive(zipFile, ZipArchiveMode.Update).ExtractToDirectory(RepositoryPath);
                zipFile.Close();
            }
            // Commit changes
            Commands.Stage(repo, "*");
            Commit commit = repo.Commit(commitMessage, signature, signature);
        }

        public void Pull(UsernamePasswordCredentials gitCredentials, Signature signature)
        {
            Repository repo = new Repository(RepositoryPath);
            // Get git credentials
            CredentialsHandler credentialsHandler = new CredentialsHandler(
                (url, usernameFromUrl, types) => gitCredentials);
            // Pull repository
            PullOptions pullOptions = new PullOptions
            {
                FetchOptions = new FetchOptions()
            };
            pullOptions.FetchOptions.CredentialsProvider = credentialsHandler;
            Signature merger = signature;
            Commands.Pull(repo, merger, pullOptions);

            // Compress changes to zip file
            File.Delete(ZipFilePath);
            using (ZipArchive zipArchive = ZipFile.Open(ZipFilePath, ZipArchiveMode.Create))
            {
                foreach (string f in DirSearch(RepositoryPath))
                {
                    zipArchive.CreateEntryFromFile(f, f.Replace(RepositoryPath + @"\", ""));
                }
            }
        }

        public void Push(UsernamePasswordCredentials gitCredentials)
        {
            Repository repo = new Repository(RepositoryPath);
            // Get git credentials
            CredentialsHandler credentialsHandler = new CredentialsHandler(
                (url, usernameFromUrl, types) => gitCredentials);
            // Pull repository
            PushOptions options = new PushOptions
            {
                CredentialsProvider = credentialsHandler
            };
            repo.Network.Push(repo.Branches["refs/heads/master"], options);
        }

        private List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            foreach (string f in Directory.GetFiles(sDir))
            {
                files.Add(f);
            }
            foreach (string d in Directory.GetDirectories(sDir))
            {
                var temp = Path.GetFileName(d);
                if (sDir != RepositoryPath || (sDir == RepositoryPath && Path.GetFileName(d) != ".git"))
                {
                    files.AddRange(DirSearch(d));
                }
            }

            return files;
        }
    }
}
