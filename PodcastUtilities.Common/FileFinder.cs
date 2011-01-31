using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace PodcastUtilities.Common
{
    public class FileFinder
    {
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        private void OnStatusUpdate(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Status, message));
        }

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            if (StatusUpdate != null)
                StatusUpdate(this, e);
        }

        public List<FileInfo> GetAllFilesInTarget(ControlFile control)
        {
            List<FileInfo> allDestFiles = new List<FileInfo>();

            string destRoot = control.DestinationRoot;

            XmlNodeList list = control.SelectNodes("podcasts/podcast");
            if (list != null)
            {
                foreach (XmlNode node in list)
                {
                    string folder = control.GetNodeText(node, "folder");
                    string pattern = control.GetNodeText(node, "pattern");

                    string destFolder = Path.Combine(destRoot, folder);
                    DirectoryInfo dest = new DirectoryInfo(destFolder);
                    if (!dest.Exists)
                        continue;

                    FileInfo[] files = dest.GetFiles(pattern);
                    if (files.Length == 0)
                        continue;

                    allDestFiles.AddRange(files);
                }
            }

            return allDestFiles;
        }

        public List<SyncItem> GetAllSourceFiles(ControlFile control, bool performDeleteOnTarget)
        {
            List<SyncItem> allSourceFiles = new List<SyncItem>();

            string sourceRoot = control.SourceRoot;
            string destRoot = control.DestinationRoot;

            XmlNodeList list = control.SelectNodes("podcasts/podcast");
            if (list != null)
            {
                foreach (XmlNode node in list)
                {
                    string folder = control.GetNodeText(node, "folder");
                    string pattern = control.GetNodeText(node, "pattern");
                    string sortField = GetSortField(control, node);
                    bool ascendingSort = !(GetSortDirection(control, node).ToLower().StartsWith("desc"));
                    int nFilesToAdd = Convert.ToInt32(control.GetNodeText(node, "number"));

                    string sourceFolder = Path.Combine(sourceRoot, folder);
                    List<SyncItem> theseFiles = GetSourceFiles(sourceFolder, pattern, nFilesToAdd, sortField, ascendingSort);

                    if (performDeleteOnTarget)
                    {
                        string destFolder = Path.Combine(destRoot, folder);
                        RemoveUnwatedFilesFromTarget(theseFiles, destFolder, pattern, false);
                    }

                    allSourceFiles.AddRange(theseFiles);
                }
            }

            return allSourceFiles;
        }

        private static string GetSortField(ControlFile control, XmlNode node)
        {
            try
            {
                return control.GetNodeText(node, "sortfield");
            }
            catch
            {
                return control.SortField;
            }
        }

        private static string GetSortDirection(ControlFile control, XmlNode node)
        {
            try
            {
                return control.GetNodeText(node, "sortdirection");
            }
            catch
            {
                return control.SortDirection;
            }
        }

        private static IEnumerable<FileInfo> GetSortedFiles(DirectoryInfo src, string pattern, string sortField, bool ascendingSort)
        {
            List<FileInfo> fileList = new List<FileInfo>(src.GetFiles(pattern));

            switch (sortField.ToLower())
            {
                case "creationtime":
                    fileList.Sort((f1, f2) => f1.CreationTime.CompareTo(f2.CreationTime));
                    break;

                default:
                    fileList.Sort((f1, f2) => f1.Name.CompareTo(f2.Name));
                    break;
            }

            if (!ascendingSort)
            {
                fileList.Reverse();
            }

            return fileList;
        }

		private static List<SyncItem> GetSourceFiles(string sourceFolder, string pattern, int nFilesToAdd, string sortField, bool ascendingSort)
        {
            List<SyncItem> retval = new List<SyncItem>();
            DirectoryInfo src = new DirectoryInfo(sourceFolder);
            var files = GetSortedFiles(src, pattern, sortField, ascendingSort); 

            int nAdded = 0;
            foreach (FileInfo file in files)
            {
                if (nFilesToAdd >= 0 && nAdded >= nFilesToAdd)
                    break;

                nAdded++;
                retval.Add(new SyncItem() { Source = file, Copied = false, DestinationPath = "" });
            }

            return retval;
        }

        private void RemoveUnwatedFilesFromTarget(List<SyncItem> sourceFiles, string destFolder, string pattern, bool whatif)
        {
            DirectoryInfo dest = new DirectoryInfo(destFolder);
            if (!dest.Exists)
                dest.Create();

            FileInfo[] files = dest.GetFiles(pattern);
            if (files.Length == 0)
                return;

            foreach (FileInfo thisFile in files)
            {
                SyncItem thisItem = new SyncItem() { Source = thisFile, Copied = false, DestinationPath = "" };
                if (sourceFiles.Find(delegate(SyncItem item) { return item.Source.Name == thisItem.Source.Name; }) == null)
                {
                    // we cannot find the file that is in the destination in the source
                    OnStatusUpdate(string.Format("Removing: {0}", thisFile.FullName));
                    if (!whatif)
                        File.Delete(thisFile.FullName);
                }
            }
        }
    }
}
