using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.Files
{
    public class ZipHelper
    {
        [Flags]
        public enum RecordingFilesPathDepth
        {
            Empty = 0,
            Node = 1, 
            Session = 2,
            Project = 4,
            Project_Session_Node = Project | Session | Node,
            Session_Node = Session | Node
        }

        private static readonly RecordingFilesPathDepth[] AllowedPathSegments = new[] 
        {
            RecordingFilesPathDepth.Project,
            RecordingFilesPathDepth.Session,
            RecordingFilesPathDepth.Node
        };

        private static readonly Dictionary<RecordingFilesPathDepth, string> PathDepthMap = new Dictionary<RecordingFilesPathDepth, string>()
        {
            { RecordingFilesPathDepth.Project, "{project}" },
            { RecordingFilesPathDepth.Session, "{session}" },
            { RecordingFilesPathDepth.Node, "{node}" }
        };


        public string CreateFilePathFormat(RecordingFilesPathDepth depth)
        {
            IEnumerable<string> filePathSegments = AllowedPathSegments.Where(segment => depth.HasFlag(segment)).Select(segment => PathDepthMap[segment]);

            string filePathFormat = String.Join("/", filePathSegments);

            return filePathFormat;
        }


        public void ZipRecordingFiles(IEnumerable<Recording> recordings, Stream stream, string filePathFormat)
        {
            using (var zipStream = new ZipOutputStream(stream))
            {
                zipStream.EnableZip64 = Zip64Option.Always;

                foreach (var recording in recordings)
                {
                    string entryPath = filePathFormat.Replace("{project}", recording.ProjectName)
                                                     .Replace("{session}", recording.SessionName)
                                                     .Replace("{node}", recording.NodeName)
                                                     .Trim(new[] { '\\', '/' });

                    foreach (var recordingFile in recording.EnumerateFiles())
                    {
                        string entry = entryPath + "/" + recordingFile.RelativePath.Replace("\\", "/").TrimStart('/');
                        
                        zipStream.PutNextEntry(entry);

                        using (var fileStream = recordingFile.OpenReadStream())
                        {
                            fileStream.CopyTo(zipStream);
                        }
                    }
                }

                zipStream.Close();
            }
        }


        public void ZipRecordingFiles(IEnumerable<Recording> recordings, Stream stream, RecordingFilesPathDepth depth)
        {
            string filePathFormat = CreateFilePathFormat(depth);

            ZipRecordingFiles(recordings, stream, filePathFormat);
        }
    }
}
