using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Studies.Models;
using UXR.Studies.ViewModels;
using UXI.Common.Extensions;

namespace UXR.Studies.Files
{
    public class RecordingFilesManager
    {
        public RecordingFilesManager()
        {
#if STAGING
            foreach (var directory in Paths.Directories.Where(dir => Directory.Exists(dir)))
            {
                Directory.Delete(directory, true);
            }
#endif

            foreach (var directory in Paths.Directories)
            {
                EnsureDirectoryExists(directory);
            }

#if STAGING
            if (Directory.Exists(Paths.TESTING_DATA_SOURCE_PATH))
            {
                CopyDirectory(Paths.TESTING_DATA_SOURCE_PATH, Paths.RECORDINGS_PATH);
            }
#endif
        }

        public static string EnsureDirectoryExists(string directoryPath)
        {
            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }

            return directoryPath;
        }


        private void CopyDirectory(string source, string destination)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(source, destination));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(source, destination), true);
            }
        }

        private string ZipFileName(string nodeName, DateTime startTime)
        {
            return Paths.DataFileName(nodeName, startTime) + ".zip";
        }


        private bool ImportRecordingFromUploads(string nodeName, DateTime startTime, string userEmail, string projectName, string sessionName)
        {
            var dataFilePath = Path.Combine(Paths.UPLOADS_PATH, ZipFileName(nodeName, startTime));

            if (File.Exists(dataFilePath))
            {
                var outputDirectory = Paths.RecordingPath(userEmail,
                    projectName,
                    sessionName,
                    nodeName,
                    startTime);

                using (ZipFile zip = new ZipFile(dataFilePath))
                {
                    Directory.CreateDirectory(outputDirectory);
                    zip.ExtractAll(outputDirectory, ExtractExistingFileAction.Throw);
                }

                return true;
            }

            return false;
        }

        public bool ImportRecordingFromUploads(string nodeName, DateTime startTime, Session session)
        {
            return ImportRecordingFromUploads(nodeName, startTime, session.Project.Owner.Email, session.Project.Name, session.Name);
        }

        public bool ImportRecordingFromUploads(string nodeName, DateTime startTime)
        {
            return ImportRecordingFromUploads(nodeName, startTime, null, null, null);
        }

        public IEnumerable<string> GetRecordingUploadFiles(string nodeName, DateTime startTime)
        {
            string fileName = Paths.DataFileName(nodeName, startTime);

            return Directory.EnumerateFiles(Paths.UPLOADS_PATH, fileName + ".*");
        }

        public void DeleteRecordingUploadFiles(string nodeName, DateTime startTime)
        {
            foreach (var uploadFile in GetRecordingUploadFiles(nodeName, startTime))
            {
                File.Delete(uploadFile);
            }
        }

        public void UpdateProjectDataLocation(Project project, string previousProjectName)
        {
            // Use temporary directory because Windows file names are case insensitive
            var currentDirectory = Paths.ProjectDirectory(project.Owner.Email, previousProjectName);
            var tempDirectory = Paths.TempDirectory(project.Name);
            var newDirectory = Paths.ProjectDirectory(project.Owner.Email, project.Name);

            if (Directory.Exists(currentDirectory))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempDirectory));
                Directory.Move(currentDirectory, tempDirectory);
                Directory.Move(tempDirectory, newDirectory);
            }
        }

        public void UpdateSessionDataLocation(Session session, string previousProjectOwnerEmail,
            string previousProjectName, string previousSessionName)
        {
            // Use temporary directory because Windows file names are case insensitive
            var currentDirectory = Paths.SessionDirectory(previousProjectOwnerEmail, previousProjectName, previousSessionName);
            var tempDirectory = Paths.TempDirectory(session.Name);
            var newDirectory = Paths.SessionDirectory(session.Project.Owner.Email, session.Project.Name, session.Name);

            if (Directory.Exists(currentDirectory))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(newDirectory));
                Directory.CreateDirectory(Path.GetDirectoryName(tempDirectory));
                Directory.Move(currentDirectory, tempDirectory);
                Directory.Move(tempDirectory, newDirectory);
            }
        }

        public void DeleteProjectData(Project project)
        {
            project.ThrowIfNull(nameof(project));

            var directory = Paths.ProjectDirectory(project.Owner.Email, project.Name);

            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }

        public void DeleteSessionData(Session session)
        {
            if (session?.Project != null)
            {
                var directory = Paths.SessionDirectory(session.Project.Owner.Email, session.Project.Name, session.Name);

                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }
            }
        }

        private IEnumerable<Recording> EnumerateRecordings(DirectoryInfo sessionDirectory)
        {
            var nodeDirs = sessionDirectory.GetDirectories();

            foreach (var nodeDir in nodeDirs)
            {
                var recordingDirs = nodeDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
                string nodeName = nodeDir.Name;

                foreach (var recordingDir in recordingDirs)
                {
                    string recordingDateTime = recordingDir.Name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

                    DateTime recordingTime;
                    if (recordingDateTime != null
                        && Formats.TryConvertFromFilePathString(Path.GetFileName(recordingDateTime), out recordingTime))
                    {
                        yield return new Recording(nodeName, recordingTime, recordingDir);
                    }
                }
            }
        }


        public bool HasSessionRecordings(Session session)
        {
            if (session != null)
            {
                string sessionName = session.Name ?? String.Empty;
                string projectName = session.Project?.Name ?? String.Empty;

                string sessionDirPath = Paths.SessionDirectory(session.Project.Owner.Email, session.Project.Name, session.Name);
                var sessionDir = new DirectoryInfo(sessionDirPath);

                return sessionDir.Exists && sessionDir.EnumerateDirectories().Any();
            }

            return false;
        }


        public IEnumerable<Recording> GetProjectRecordings(Project project)
        {
            project.ThrowIfNull(nameof(project));

            return project.Sessions
                          .Where(s => s != null)
                          .SelectMany(s => GetSessionRecordings(s))
                          .ToList();
        }


        public IEnumerable<Recording> GetSessionRecordings(Session session)
        {
            session.ThrowIfNull(nameof(session));

            string sessionName = session.Name ?? String.Empty;
            string projectName = session.Project.Name ?? String.Empty;

            string sessionDirPath = Paths.SessionDirectory(session.Project.Owner.Email, session.Project.Name, session.Name);
            var sessionDir = new DirectoryInfo(sessionDirPath);

            if (sessionDir.Exists)
            {
                var recordings = new List<Recording>();

                foreach (var recording in EnumerateRecordings(sessionDir))
                {
                    recording.SessionName = sessionName;
                    recording.ProjectName = projectName;

                    recordings.Add(recording);
                }

                return recordings;
            }

            return Enumerable.Empty<Recording>();
        }


        public IEnumerable<Recording> GetUnassignedRecordings()
        {
            string unassignedRecordingsDirPath = Paths.UNASSIGNED_RECORDINGS_PATH;
            var unassignedRecordingsDir = new DirectoryInfo(unassignedRecordingsDirPath);

            if (unassignedRecordingsDir.Exists)
            {
                return EnumerateRecordings(unassignedRecordingsDir).ToList();
            }

            return Enumerable.Empty<Recording>();
        }

        private string GetNextFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string pathName = Path.GetDirectoryName(fileName);
            string fileNameOnly = Path.Combine(pathName, Path.GetFileNameWithoutExtension(fileName));
            int i = 0;

            // If the file or directory exists, keep trying until we find unique one
            while (File.Exists(fileName) || Directory.Exists(fileName))
            {
                i += 1;
                fileName = $"{fileNameOnly} ({i}){extension}";
            }
            return fileName;
        }


        public bool AssignRecordingToSession(Session session, string recordingNodeName, DateTime recordingStartTime)
        {
            var oldDirectory = Paths.RecordingPath(null, null, null, recordingNodeName, recordingStartTime);

            if (session != null && Directory.Exists(oldDirectory))
            {
                var newDirectory = Paths.RecordingPath(session.Project.Owner.Email, session.Project.Name, session.Name, recordingNodeName, recordingStartTime);

                if (Directory.Exists(newDirectory))
                {
                    //newDirectory = GetNextFileName(newDirectory);
                    return false;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(newDirectory));
                Directory.Move(oldDirectory, newDirectory);

                return true;
            }
            return false;
        }
    }
}
