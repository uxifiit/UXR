using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UXI.Common.Extensions;

namespace UXR.Studies.Extensions
{
    static class FileStreamResultControllerEx
    {
        public static ActionResult StreamFileResult(this Controller controller, string filename, string mimeType, Action<Stream> writeAction)
        {
            controller.ThrowIfNull(nameof(controller));
            filename.ThrowIf(String.IsNullOrWhiteSpace, nameof(filename));
            writeAction.ThrowIfNull(nameof(writeAction));

            controller.Response.BufferOutput = false;

            var serverPipe = new AnonymousPipeServerStream(PipeDirection.Out);

            Task.Run(() =>
            {
                using (serverPipe)
                {
                    writeAction.Invoke(serverPipe);
                }
            });

            var clientPipe = new AnonymousPipeClientStream(PipeDirection.In, serverPipe.ClientSafePipeHandle);

            return new FileStreamResult(clientPipe, mimeType)
            {
                FileDownloadName = filename
            };
        }
    }
}
