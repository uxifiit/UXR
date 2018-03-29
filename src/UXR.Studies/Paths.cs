using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using UXR.Studies.Files;

namespace UXR.Studies
{
    public static class Paths
    {

        public static readonly string DATA_PATH = ((NameValueCollection)ConfigurationManager.GetSection("DeploymentConfig"))["DataPath"];

        public static readonly string UPLOADS_PATH = Path.Combine(DATA_PATH, "Uploads");

        public static readonly string RECORDINGS_PATH = Path.Combine(DATA_PATH, "Recordings");

        public static readonly string TEMP_PATH = Path.Combine(DATA_PATH, "Temp");

#if STAGING
        public static readonly string TESTING_DATA_SOURCE_PATH = Path.Combine(DATA_PATH, "TestingDataSource");
#endif

        public static readonly string UNASSIGNED_RECORDINGS_PATH = Path.Combine(RECORDINGS_PATH, "Unassigned");

        public static readonly string PROJECT_RECORDINGS_PATH = Path.Combine(RECORDINGS_PATH, "Projects");

        public static IEnumerable<string> Directories
        {
            get
            {
                yield return UPLOADS_PATH;
                yield return TEMP_PATH;
                yield return RECORDINGS_PATH;
                yield return UNASSIGNED_RECORDINGS_PATH;
                yield return PROJECT_RECORDINGS_PATH;
            }
        }

        public static string DataFileName(string nodeName, DateTime startTime)
        {
            return nodeName + "_" + Formats.ConvertToFilePathValidString(startTime);
        }

        public static string ProjectDirectory(string userEmail, string projectName)
        {
            return Path.Combine(PROJECT_RECORDINGS_PATH, userEmail, projectName);
        }

        public static string SessionDirectory(string userEmail, string projectName, string sessionName)
        {
            return Path.Combine(ProjectDirectory(userEmail, projectName), sessionName);
        }

        public static string TempDirectory(string directoryName)
        {
            return Path.Combine(TEMP_PATH, directoryName);
        }

        public static string RecordingPath(string userEmail, string projectName, string sessionName, string nodeName, DateTime recordingStartTime)
        {
            if (String.IsNullOrWhiteSpace(sessionName))
            {
                return Path.Combine(UNASSIGNED_RECORDINGS_PATH, nodeName, Formats.ConvertToFilePathValidString(recordingStartTime));
            }
            else
            {
                return Path.Combine(SessionDirectory(userEmail, projectName, sessionName), nodeName, Formats.ConvertToFilePathValidString(recordingStartTime));
            }
        }
    }
}
