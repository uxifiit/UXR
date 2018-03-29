using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.Files
{
    public class Recording
    {
        public DirectoryInfo _directory;

        public Recording(string nodeName, DateTime startTime, DirectoryInfo directory)
        {
            NodeName = nodeName;
            StartTime = startTime;
            _directory = directory;
        }

        public string NodeName { get; }
        public DateTime StartTime { get; }
        public string ProjectName { get; internal set; }
        public string SessionName { get; internal set; }

        public IEnumerable<RecordingFile> EnumerateFiles()
        {
            return _directory.EnumerateFiles("*", SearchOption.AllDirectories)
                             .Select(file => new RecordingFile(file, _directory.Parent));
        }
    }


    public class RecordingFile
    {
        private readonly FileInfo _file;
        private readonly DirectoryInfo _root;

        public RecordingFile(FileInfo file, DirectoryInfo rootDirectory)
        {
            _file = file;
            _root = rootDirectory;
        }

        public string RelativePath
        {
            get
            {
                if (_file.FullName.StartsWith(_root.FullName))
                {
                    return _file.FullName.Substring(_root.FullName.Length);
                }
                else
                {
                    return _file.Name;
                }
            }
        }

        public Stream OpenReadStream()
        {
            return _file.Exists
                 ? _file.OpenRead()
                 : Stream.Null;
        }
    }
}
