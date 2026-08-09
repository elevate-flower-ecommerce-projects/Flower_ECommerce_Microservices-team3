using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(
                                 IFormFile file,
                                 string folder,
                                 CancellationToken cancellationToken = default);
    }
}
