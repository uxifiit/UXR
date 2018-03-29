using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UXR.Studies.Api.Entities.Nodes;
using UXR.Studies.Api.Entities.Sessions;

namespace UXR.Studies.Client
{
    public interface IUXRClient : IDisposable
    {
        Uri EndpointUri { get; set; }

        event EventHandler<Uri> EndpointUriChanged;

        Task<bool> CheckConnectionAsync();
        Task<bool> CheckConnectionAsync(Uri endpointUri);
        Task<IEnumerable<SessionInfo>> GetCurrentSessionsAsync();
        Task<IEnumerable<string>> GetUploadedSessionRecordingFilesAsync(int nodeId, DateTime startTime);
        Task<bool> SaveSessionRecordingAsync(int nodeId, DateTime startTime, int? sessionId);
        Task<NodeIdInfo> UpdateNodeStatusAsync(NodeStatusUpdate status, CancellationToken cancellationToken);
        Task<bool> UploadSessionRecordingFileAsync(int nodeId, DateTime startTime, FileStream file, IProgress<int> progress, CancellationToken cancellationToken);
    }
}
