using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICloudflareUploadService
    {
        Task<(string StreamId, string PlaybackUrl, string Mp4Url)> UploadVideoAsync(IFormFile videoFile);
    }
}
