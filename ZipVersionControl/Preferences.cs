using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using ZipVersionControlLib;

namespace ZipVersionControl
{
    class Preferences
    {
        public static List<Profile> Profiles { get; private set; } = new List<Profile>();
        public static string GitName { get; set; } = "";
        public static string GitEmail { get; set; } = "";

        public static void Load()
        {
            // Reset profiles list
            Profiles.Clear();
            // Save file path to variable
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZipVersionControl\\preferences.xml");
            // Check if AppData directory exists
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZipVersionControl")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZipVersionControl"));
            }
            // Load xml preferences file

            if (File.Exists(filePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                // Add profiles to profiles list
                XmlNodeList profileNodes = doc.DocumentElement.SelectNodes("Profiles/Profile");
                foreach (XmlNode profileNode in profileNodes)
                {
                    string name = profileNode.Attributes["Name"].Value;
                    string zipFilePath = profileNode.Attributes["ZipFilePath"].Value;
                    string repositoryPath = profileNode.Attributes["RepositoryPath"].Value;
                    string username = profileNode.Attributes["Username"].Value;
                    byte[] password = Convert.FromBase64String(profileNode.Attributes["EncryptedPassword"].Value);
                    DateTimeOffset latestCommitTime = DateTimeOffset.Parse(profileNode.Attributes["LatestCommitTime"].Value);
                    byte[] zipFileHash = Convert.FromBase64String(profileNode.Attributes["ZipFileHash"].Value);
                    Profiles.Add(new Profile() { ProfileName = name, ZipFilePath = zipFilePath, RepositoryPath = repositoryPath, Username = username, Password = password, LatestCommitTime = latestCommitTime, ZipFileHash = zipFileHash });
                }
                // Initialize settings
                GitName = doc.DocumentElement.SelectSingleNode("Options/GitName").InnerText;
                GitEmail = doc.DocumentElement.SelectSingleNode("Options/GitEmail").InnerText;
            }
        }

        public static void Save()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZipVersionControl\\preferences.xml");
            // Save profiles to file
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null); // Create xml declaration
            doc.AppendChild(docNode);
            XmlNode zipVersionControlNode = doc.CreateElement("ZipVersionControl"); // Create zip version control node
            doc.AppendChild(zipVersionControlNode);
            XmlNode profilesNode = doc.CreateElement("Profiles"); // Create profiles node
            zipVersionControlNode.AppendChild(profilesNode);
            for (int i = 0; i <= Profiles.Count() - 1; i++)
            {
                XmlNode profileNode = doc.CreateElement("Profile"); // Create profile node
                profileNode.Attributes.Append(doc.CreateAttribute("Name")).Value = Profiles[i].ProfileName;
                profileNode.Attributes.Append(doc.CreateAttribute("ZipFilePath")).Value = Profiles[i].ZipFilePath;
                profileNode.Attributes.Append(doc.CreateAttribute("RepositoryPath")).Value = Profiles[i].RepositoryPath;
                profileNode.Attributes.Append(doc.CreateAttribute("Username")).Value = Profiles[i].Username;
                profileNode.Attributes.Append(doc.CreateAttribute("EncryptedPassword")).Value = Convert.ToBase64String(Profiles[i].Password);
                profileNode.Attributes.Append(doc.CreateAttribute("LatestCommitTime")).Value = Profiles[i].LatestCommitTime.ToString();
                profileNode.Attributes.Append(doc.CreateAttribute("ZipFileHash")).Value = Convert.ToBase64String(Profiles[i].ZipFileHash);
                profilesNode.AppendChild(profileNode);
            }
            XmlNode optionsNode = doc.CreateElement("Options"); // Create options node
            zipVersionControlNode.AppendChild(optionsNode);
            XmlNode gitNameNode = doc.CreateElement("GitName"); // Create git name node
            optionsNode.AppendChild(gitNameNode);
            gitNameNode.InnerText = GitName;
            XmlNode gitEmailNode = doc.CreateElement("GitEmail"); // Create git email node
            optionsNode.AppendChild(gitEmailNode);
            gitEmailNode.InnerText = GitEmail;
            doc.Save(filePath);
        }
    }
}
