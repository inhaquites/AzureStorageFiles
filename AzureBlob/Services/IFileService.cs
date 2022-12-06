using AzureBlob.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureBlob.Services
{
    public interface IFileService
    {
        Task<string> UploadBase64Image(string base64Image, string container, string connection);

        Task<List<string>> GetFiles(string container, string connection, GetFilesModel fileModel);

        Task<bool> DeleteFile(string container, string connection, DeleteFileModel deleteFileModel);
    }
}
