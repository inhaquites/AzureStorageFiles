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
        public async Task<string> UploadBase64Image(string fileName, string base64Image, string container, string connection) 
        {
            var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(base64Image, "");

            byte[] imageBytes = Convert.FromBase64String(data);

            var blobClient = new BlobClient(connection, container, fileName);

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
                        listaArquivos.Add(string.Format("https://anexos2022.blob.core.windows.net/{0}/{1}/{2}", fileModel.FormId, fileModel.FieldId, desm[2]));
                    }
                }
            }
            //https://anexos2022.blob.core.windows.net/formularios/1/333/MeuArquivo222.jpg
            return listaArquivos;
        }

        public async Task<bool> DeleteFile(string container, string connection, DeleteFileModel deleteFileModel)
        {
            var blobContainerClient = new BlobContainerClient(connection, container);
            
            var path = string.Format("{0}/{1}/{2}", deleteFileModel.FormId, deleteFileModel.FieldId, deleteFileModel.FileName);
            
            var blob = blobContainerClient.GetBlobClient(path);

            return await blob.DeleteIfExistsAsync();
        }
    }
}
