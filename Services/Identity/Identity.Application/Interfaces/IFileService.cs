using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Application.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default);
        Task<string> UploadBase64Async(string base64Data, string folderName, CancellationToken cancellationToken = default);
        Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
        string GetPublicUrl(string? relativePathOrUrl);
    }
}
