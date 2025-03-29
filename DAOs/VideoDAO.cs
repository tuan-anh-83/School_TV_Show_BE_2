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

        public async Task<List<VideoHistory>> GetAllVideosAsync()
        {
            return await _context.VideoHistories
                .Where(vh => vh.Status)
                .Include(vh => vh.Program)
                .ToListAsync();
        }

        public async Task<VideoHistory?> GetVideoByIdAsync(int videoHistoryId)
        {
            return await _context.VideoHistories
                .Include(vh => vh.Program)
                .FirstOrDefaultAsync(vh => vh.VideoHistoryID == videoHistoryId && vh.Status);
        }

        public async Task<VideoHistory?> GetLatestLiveStreamByProgramIdAsync(int programId)
        {
            return await _context.VideoHistories
                .Where(vh => vh.ProgramID == programId && vh.Type == "Live" && vh.Status)
                .OrderByDescending(vh => vh.CreatedAt)
                .FirstOrDefaultAsync();
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

        public async Task<bool> UpdateVideoAsync(VideoHistory videoHistory)
        {
            var existingVideo = await GetVideoByIdAsync(videoHistory.VideoHistoryID);
            if (existingVideo == null)
                return false;

            existingVideo.URL = videoHistory.URL;
            existingVideo.Type = videoHistory.Type;
            existingVideo.Description = videoHistory.Description;
            existingVideo.ProgramID = videoHistory.ProgramID;
            existingVideo.UpdatedAt = DateTime.UtcNow;

            try
            {
                _context.VideoHistories.Update(existingVideo);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating video: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteVideoAsync(int videoHistoryId)
        {
            var videoHistory = await GetVideoByIdAsync(videoHistoryId);
            if (videoHistory == null)
                return false;

            videoHistory.Status = false;

            try
            {
                _context.VideoHistories.Update(videoHistory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting video: {ex.Message}");
                return false;
            }
        }

        public async Task<int> CountByStatusAsync(bool status)
        {
            return await _context.VideoHistories.CountAsync(v => v.Status == status);
        }

        public async Task<(int totalViews, int totalLikes)> GetTotalViewsAndLikesAsync()
        {
            int totalViews = await _context.VideoHistories.SumAsync(v => v.VideoViews.Count);
            int totalLikes = await _context.VideoHistories.SumAsync(v => v.VideoLikes.Count);
            return (totalViews, totalLikes);
        }

        public async Task<int> CountByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.VideoHistories.CountAsync(v => v.CreatedAt >= startDate && v.CreatedAt <= endDate);
        }

        public async Task<int> CountTotalVideosAsync()
        {
            return await _context.VideoHistories.CountAsync();
        }
    }
}
