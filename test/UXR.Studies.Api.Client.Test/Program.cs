using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UXI.Common.Extensions;

namespace UXR.Studies.Client.Test
{
    class Program
    {
        private static readonly DateTime starttime = DateTime.Now;

        static void Main(string[] args)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                using (UXRClient client = new UXRClient(new Uri("http://localhost:15185/")))
                {
                    TestAsync(client, cts.Token).Forget();
                    Console.ReadLine();
                }
            }
        }

        private static async Task TestAsync(UXRClient client, CancellationToken cancellationToken)
        {
            Console.Write("Checking connection: ");
            bool connected = await client.CheckConnectionAsync();
            Console.WriteLine(connected ? "OK" : "Failed");


            Console.Write("Updating node status: ");
            var info = await client.UpdateNodeStatusAsync(new Api.Entities.Nodes.NodeStatusUpdate()
            {
                Name = "UX Class 01",
                Recording = new Api.Entities.Sessions.SessionRecordingUpdate()
                {
                    SessionId = null,
                    SessionName = "Recording",
                    StartedAt = DateTime.Now,
                    Streams = new List<string>() { "ET", "EXTEV", "WCV" } 
                }
            }, CancellationToken.None);

            Console.WriteLine(info != null ? $"OK {info.Id.ToString()}" : "Failed");

            if (info != null)
            {
                Console.WriteLine("Uploading data: ");

                var data = Directory.GetFiles("data");
                foreach (var dataFile in data)
                {
                    Console.WriteLine("Uploading: " + dataFile);
                    Progress<int> progress = new Progress<int>(i => Console.Write($"\rProgress: {i.ToString("000")}"));
                    bool uploadedFile = await client.UploadSessionRecordingFileAsync(info.Id, starttime, File.OpenRead(dataFile), progress, cancellationToken);
                    Console.WriteLine();
                    Console.WriteLine("Uploaded: " + uploadedFile);
                }
                Console.WriteLine("Uploaded files:");
                IEnumerable<string> files = await client.GetUploadedSessionRecordingFilesAsync(info.Id, starttime);
                files.ForEach(f => Console.WriteLine(f));

                bool uploaded = files.All(f => data.Any(d => d.EndsWith(f)));

                if (uploaded)
                {
                    Console.Write("Saving data: ");
                    bool saved = await client.SaveSessionRecordingAsync(info.Id, starttime, null);
                    Console.WriteLine(saved ? "OK" : "Failed");
                }
            }
        }
    }
}
