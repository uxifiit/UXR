using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using UXR.Studies.Api.Entities;
using UXR.Studies.Api.Files.Transfer;
using UXR.Studies.Files;
using UXR.Studies.Models;
using UXR.Studies.Models.Queries;

namespace UXR.Studies.Api.Controllers
{
    [RoutePrefix(ApiRoutes.Node.Recording.PREFIX)]
    public class NodeRecordingApiController : ApiController
    {
        private readonly StudiesDatabase _database;
        private readonly RecordingFilesManager _recordings;


        public NodeRecordingApiController(StudiesDatabase database, RecordingFilesManager recordings)
        {
            _database = database;
            _recordings = recordings;
        }
        

        [HttpPost]
        [MimeMultipart]
        [Route(ApiRoutes.Node.Recording.ACTION_UPLOAD)]
        public async Task<IHttpActionResult> UploadRecordingData(int nodeId, string startTime)
        {
            DateTime startDateTime;
            if (Request.Content.IsMimeMultipartContent() 
                && ApiRoutes.Node.Recording.TryParseDateTimeParameter(startTime, out startDateTime))
            {
                var node = _database.Nodes
                                    .FilterById(nodeId)
                                    .SingleOrDefault();

                if (node != null)
                {
                    string uploadPath = Paths.UPLOADS_PATH;
                    string fileName = Paths.DataFileName(node.Name, startDateTime);

                    try
                    {
                        FileMultipartFormProvider multipartFormDataStreamProvider = new FileMultipartFormProvider(uploadPath)
                        {
                            AllowedExtensions = new List<string> { ".zip" },
                            AllowedExtensionRegexes = new List<Regex> { new Regex(@"^\.z[0-9]{2,}$") },
                            FileName = fileName
                        };

                        // Read the MIME multipart asynchronously 
                        await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);

                        return Ok();
                    }
                    catch (Exception e)
                    {
                        return Content(HttpStatusCode.InternalServerError, e);
                    }
                }

                return StatusCode(HttpStatusCode.Forbidden);
            }

            return BadRequest();
        }


        // Unpacks the zip files for a recording if the files were previously updated
        // Call with:
        // curl -H "Content-Type: application/json" -X POST -d "" "http://localhost:15185/api/studies/node/1/recording/2017-12-19T14_00_00/save/1"
        [HttpPost]
        [Route(ApiRoutes.Node.Recording.ACTION_SAVE)]
        public IHttpActionResult SaveRecording(int nodeId, string startTime, int? sessionId = null)
        {
            DateTime startDateTime;
            if (ApiRoutes.Node.Recording.TryParseDateTimeParameter(startTime, out startDateTime))
            {
                var node = _database.Nodes
                                    .FilterById(nodeId)
                                    .SingleOrDefault();

                if (node != null)
                {
                    Session session = null;
                    bool imported = false;

                    if (sessionId.HasValue)
                    {
                        session = _database.Sessions
                                           .FilterById(sessionId.Value)
                                           .AsDbQuery()
                                           .Include(nameof(Session.Project))
                                           .Include(nameof(Session.Project) + "." + nameof(Project.Owner))
                                           .SingleOrDefault();
                    }

                    if (session != null)
                    {
                        imported = _recordings.ImportRecordingFromUploads(node.Name, startDateTime, session);
                    }
                    else
                    {
                        imported = _recordings.ImportRecordingFromUploads(node.Name, startDateTime);
                    }

                    if (imported)
                    {
                        _recordings.DeleteRecordingUploadFiles(node.Name, startDateTime);
                        return Ok();
                    }

                    return NotFound();
                }

                return StatusCode(HttpStatusCode.Forbidden);
            }
            return BadRequest();
        }


        [HttpGet]
        [Route(ApiRoutes.Node.Recording.ACTION_LIST)]
        public IHttpActionResult ListRecordingUploads(int nodeId, string startTime)
        {
            DateTime startDateTime;
            if (ApiRoutes.Node.Recording.TryParseDateTimeParameter(startTime, out startDateTime))
            {
                var node = _database.Nodes
                                    .FilterById(nodeId)
                                    .SingleOrDefault();

                if (node != null)
                {
                    var files = _recordings.GetRecordingUploadFiles(node.Name, startDateTime);

                    var extensions = files.Select(System.IO.Path.GetExtension);

                    return Ok(extensions.ToList());
                }

                return NotFound();
            }

            return BadRequest();
        }
    }
}
