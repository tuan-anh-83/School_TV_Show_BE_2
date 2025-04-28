using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CloudFlareService
{
    public interface ICloudService
    {
        Task<string> UploadVideoAsync(IFormFile file);
    }
}
