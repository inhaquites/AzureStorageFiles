using Azure.Storage.Blobs;
using AzureBlob.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AzureBlob.Services
{
    public class FileService : IFileService
    {
        public async Task<string> UploadBase64Image(string base64Image, string container, string connection) 
        {
            var fileName = "MeuArquivo.jpg";

            var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(base64Image, "");

            byte[] imageBytes = Convert.FromBase64String(data);

            var blobClient = new BlobClient(connection, container, "1/333/" + fileName);

            using (var stream = new MemoryStream(imageBytes))
            {
                blobClient.Upload(stream);                
            }

            return blobClient.Uri.AbsoluteUri;
        }

		public async Task<List<string>> GetFiles(string container, string connection, GetFilesModel fileModel)
        {
            var blobContainerClient = new BlobContainerClient(connection, container);

            var resultSegment = blobContainerClient.GetBlobsAsync().AsPages(default);
            List<string> listaArquivos = new List<string>();
            await foreach (var blobPage in resultSegment)
            {
                var storage = blobPage.Values;
                foreach (var item in storage)
                {
                    var desm = item.Name.Split("/");
                    if (desm[0].Equals(fileModel.FormId) && desm[1].Equals(fileModel.FieldId))
                    {
                        listaArquivos.Add(desm[2]);
                    }
                }
            }

            return listaArquivos;
        }
	}
}
