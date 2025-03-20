using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class VideoDAO
    {
        private static VideoDAO instance = null;
        private readonly DataContext _context;

        private VideoDAO()
        {
            _context = new DataContext();
        }

        public static VideoDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new VideoDAO();
                }
                return instance;
            }
        }

        public async Task<bool> AddVideoAsync(VideoHistory videoHistory)
        {
            try
            {
                videoHistory.CreatedAt = DateTime.UtcNow;
                videoHistory.UpdatedAt = DateTime.UtcNow;
                videoHistory.StreamAt = DateTime.UtcNow;
                videoHistory.Status = true;

                _context.VideoHistories.Add(videoHistory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding video: {ex.Message}");
                return false;
            }
        }




    }
}
