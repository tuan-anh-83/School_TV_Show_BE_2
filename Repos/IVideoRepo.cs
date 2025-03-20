using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public interface IVideoRepo
    {
        Task<bool> AddVideoAsync(VideoHistory videoHistory);
    }
}
