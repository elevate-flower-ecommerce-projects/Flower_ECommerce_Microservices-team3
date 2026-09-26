using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Services
{
    public class LocalFileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalFileService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRootPath, folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext))
            {
                ext = file.ContentType switch
                {
                    "image/png" => ".png",
                    "image/webp" => ".webp",
                    "image/gif" => ".gif",
                    _ => ".jpg"
                };
            }

            var uniqueFileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream, cancellationToken);
            }

            return GetPublicUrl($"/{folderName}/{uniqueFileName}");
        }

        public async Task<string> UploadBase64Async(string base64Data, string folderName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(base64Data))
                return string.Empty;

            var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRootPath, folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var ext = ".jpg";
            var rawData = base64Data;

            if (base64Data.Contains(";base64,"))
            {
                var parts = base64Data.Split(";base64,");
                if (parts[0].Contains("png")) ext = ".png";
                else if (parts[0].Contains("webp")) ext = ".webp";
                else if (parts[0].Contains("gif")) ext = ".gif";
                rawData = parts[1];
            }

            var bytes = Convert.FromBase64String(rawData);
            var uniqueFileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);

            return GetPublicUrl($"/{folderName}/{uniqueFileName}");
        }

        public Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return Task.CompletedTask;

            try
            {
                string path;
                if (Uri.TryCreate(fileUrl, UriKind.Absolute, out var uri))
                {
                    path = uri.AbsolutePath;
                }
                else
                {
                    path = fileUrl;
                }

                path = path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);

                var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var fullPath = Path.Combine(webRootPath, path);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch
            {
             
            }

            return Task.CompletedTask;
        }

        public string GetPublicUrl(string? relativePathOrUrl)
        {
            if (string.IsNullOrWhiteSpace(relativePathOrUrl))
                return string.Empty;

            var path = relativePathOrUrl.Trim();

            // Check if it's already an absolute HTTP/HTTPS URL
            if (Uri.TryCreate(path, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                // If it points to internal docker name or internal port, rewrite to public gateway
                if (uri.Host.Equals("identity-api", StringComparison.OrdinalIgnoreCase) ||
                    uri.Host.StartsWith("flower_", StringComparison.OrdinalIgnoreCase) ||
                    uri.Port == 5022 ||
                    (uri.Port == 8080 && !uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) && !uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase)))
                {
                    path = uri.PathAndQuery;
                }
                else if (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) && uri.Port == 5022)
                {
                    path = uri.PathAndQuery;
                }
                else
                {
                    return path;
                }
            }

            if (!path.StartsWith('/'))
            {
                path = "/" + path;
            }

            var request = _httpContextAccessor.HttpContext?.Request;
            string scheme = "http";
            string host = "localhost:8080";

            if (request != null)
            {
                scheme = request.Headers["X-Forwarded-Proto"].FirstOrDefault()
                         ?? (request.IsHttps ? "https" : "http");

                var forwardedHost = request.Headers["X-Forwarded-Host"].FirstOrDefault();
                host = !string.IsNullOrWhiteSpace(forwardedHost)
                    ? forwardedHost
                    : (request.Host.HasValue ? request.Host.Value : "localhost:8080");

                if (string.IsNullOrWhiteSpace(host) ||
                    host.StartsWith("identity-api", StringComparison.OrdinalIgnoreCase) ||
                    host.StartsWith("flower_", StringComparison.OrdinalIgnoreCase))
                {
                    host = "localhost:8080";
                }
            }

            return $"{scheme}://{host}{path}";
        }
    }
}
