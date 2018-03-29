using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace UXR.Studies.Api.Files.Transfer
{
    public class FileMultipartFormProvider : MultipartFormDataStreamProvider
    {
        public FileMultipartFormProvider(string rootPath) 
            : base(rootPath)
        {
        }

        private static string GetFileNameFromHeaders(HttpContentHeaders headers)
        {
            return headers.ContentDisposition.FileName.Trim('"');
        }

        public string FileName { get; set; }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            if (headers != null 
                && headers.ContentDisposition != null)
            {
                if (FileName == null)
                {
                    return GetFileNameFromHeaders(headers);
                }
                else
                {
                    return FileName + Path.GetExtension(GetFileNameFromHeaders(headers));
                }
            }

            return base.GetLocalFileName(headers);
        }

        public List<string> AllowedExtensions { get; set; }

        public List<Regex> AllowedExtensionRegexes { get; set; }

        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            var fileExtension = Path.GetExtension(GetFileNameFromHeaders(headers));

            return (AllowedExtensions == null && AllowedExtensionRegexes == null) 
                || AllowedExtensions.Any(i => i.Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase)) 
                || AllowedExtensionRegexes.Any(regex => regex.Match(fileExtension).Success) 
                ? base.GetStream(parent, headers) 
                : Stream.Null;
        }
    }
}
