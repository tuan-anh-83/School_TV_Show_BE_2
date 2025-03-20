using BOs.Models;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public class VideoRepo : IVideoRepo
    {
        public async Task<bool> AddVideoAsync(VideoHistory video)
        {
            return await VideoDAO.Instance.AddVideoAsync(video);
        }
    }
}
