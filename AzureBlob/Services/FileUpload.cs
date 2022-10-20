using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AzureBlob.Services
{
    public class FileUpload
    {
        public string UploadBase64Image(string base64Image, string container, string connection) 
        {
            var fileName = "MeuArquivo2.jpg";

            var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(base64Image, "");

            byte[] imageBytes = Convert.FromBase64String(data);

            var blobClient = new BlobClient(connection, container, "folder/" + fileName);

            using (var stream = new MemoryStream(imageBytes))
            {
                blobClient.Upload(stream);                
            }

            return blobClient.Uri.AbsoluteUri;
        }
    }
}
