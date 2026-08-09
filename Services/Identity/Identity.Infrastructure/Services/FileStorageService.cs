using System;
using System.Collections.Generic;
using System.Text;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Identity.Infrastructure.Services
{
    public sealed class FileStorageService : IFileStorageService
    {
        private readonly string _rootPath;

        public FileStorageService(IConfiguration configuration)
        {
            _rootPath = configuration["FileStorage:RootPath"]
                ?? throw new InvalidOperationException(
                    "FileStorage:RootPath is not configured.");
        }

        public async Task<string> UploadAsync(
            IFormFile file,
            string folder,
            CancellationToken cancellationToken)
        {
            var folderPath = Path.Combine(_rootPath, folder);

            Directory.CreateDirectory(folderPath);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(folderPath, fileName);

            await using var stream = new FileStream(
                filePath,
                FileMode.Create);

            await file.CopyToAsync(
                stream,
                cancellationToken);

            return Path.Combine(folder, fileName)
                .Replace("\\", "/");
        }
    }
}
