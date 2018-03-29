using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UXI.Common;
using UXI.Common.Extensions;
using UXI.Common.Web;
using UXI.Common.Web.Extensions;
using UXR.Studies.Api;
using UXR.Studies.Api.Entities;
using UXR.Studies.Api.Entities.Nodes;
using UXR.Studies.Api.Entities.Sessions;

namespace UXR.Studies.Client
{
    public class UXRClient : DisposableBase, IUXRClient, IDisposable
    {
        private HttpClient _client;

        private static readonly JsonMediaTypeFormatter mediaType = new JsonMediaTypeFormatter
        {
            SerializerSettings =
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                DateParseHandling = DateParseHandling.DateTime
            }
        };

      
        public UXRClient(Uri endpointUri)
        {
            _client = CreateClient(endpointUri);
        }


        /// <summary>
        /// Creates <see cref="HttpClient"/> instance with given endpointUri as <see cref="HttpClient.BaseAddress"/>.
        /// </summary>
        /// <returns></returns>
        private static HttpClient CreateClient(Uri endpointUri)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = endpointUri;
            client.DisableCaching();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }


        public Uri EndpointUri
        {
            get
            {
                return _client.BaseAddress;
            }
            set
            {
                value.ThrowIfNull(nameof(EndpointUri));

                if (_client.BaseAddress != value)
                {
                    var oldClient = ObjectEx.GetAndReplace(ref _client, CreateClient(value));

                    if (oldClient != null)
                    {
                        oldClient.CancelPendingRequests();
                        oldClient.Dispose();
                    }

                    EndpointUriChanged?.Invoke(this, value);
                }
            }
        }

        public event EventHandler<Uri> EndpointUriChanged;


        public Task<bool> CheckConnectionAsync()
        {
            return CheckConnectionAsync(_client);
        }


        public Task<bool> CheckConnectionAsync(Uri endpointUri)
        {
            using (var client = CreateClient(endpointUri))
            {
                return CheckConnectionAsync(client);
            }
        }


        private static async Task<bool> CheckConnectionAsync(HttpClient client)
        {
            client.ThrowIfNull(nameof(client));

            try
            {
                var response = await client.GetAsync(ApiRoutes.Status.ResolveIndexRoute());

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return false;
        }


        public async Task<NodeIdInfo> UpdateNodeStatusAsync(NodeStatusUpdate status, CancellationToken cancellationToken)
        {
            status.ThrowIfNull(nameof(status));

            try
            {
                var response = await _client.PostAsync(ApiRoutes.Node.ResolveUpdateRoute(), status, mediaType, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<NodeIdInfo>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return null;
        }


        public async Task<IEnumerable<SessionInfo>> GetCurrentSessionsAsync()
        {
            try
            {
                string json = await _client.GetStringAsync(ApiRoutes.Session.ResolveCurrentSessionsRoute());

                if (String.IsNullOrWhiteSpace(json) == false)
                {
                    return JsonConvert.DeserializeObject<List<SessionInfo>>(json);
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return Enumerable.Empty<SessionInfo>();
        }


        public async Task<bool> SaveSessionRecordingAsync(int nodeId, DateTime startTime, int? sessionId)
        {
            try
            {
                string route;
                if (sessionId.HasValue)
                {
                    route = ApiRoutes.Node.Recording.ResolveSaveRoute(nodeId, startTime, sessionId.Value);
                }
                else
                {
                    route = ApiRoutes.Node.Recording.ResolveSaveUnassignedRoute(nodeId, startTime);
                }

                var response = await _client.PostAsync(route, new StringContent(String.Empty));

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return false;
        }


        public async Task<bool> UploadSessionRecordingFileAsync(int nodeId, DateTime startTime, FileStream file, IProgress<int> progress, CancellationToken cancellationToken)
        {
            try
            {
                var form = new MultipartFormDataContent();

                var streamContent = new ProgressStreamContent(file, progress, cancellationToken);

                form.Add(streamContent, "\"file\"", $"{Path.GetFileName(file.Name)}");

                string uploadActionRoute = ApiRoutes.Node.Recording.ResolveUploadRoute(nodeId, startTime);
                var response = await _client.PostAsync(uploadActionRoute, form, cancellationToken);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return false;
        }


        public async Task<IEnumerable<string>> GetUploadedSessionRecordingFilesAsync(int nodeId, DateTime startTime)
        {
            try
            {
                string json = await _client.GetStringAsync(ApiRoutes.Node.Recording.ResolveListRoute(nodeId, startTime));

                List<string> files = JsonConvert.DeserializeObject<List<string>>(json);

                return files;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return Enumerable.Empty<string>();
        }

        #region IDisposable members

        private bool _disposed = false;

        protected override void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {
                    _client.CancelPendingRequests();
                    _client.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
