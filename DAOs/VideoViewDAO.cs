using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAOs
{
    public class VideoViewDAO
    {
        private static VideoViewDAO instance = null;
        private readonly DataContext _context;

        private VideoViewDAO()
        {
            _context = new DataContext();
        }

        public static VideoViewDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new VideoViewDAO();
                }
                return instance;
            }
        }

        public async Task<List<VideoView>> GetAllVideoViewsAsync()
        {
            return await _context.VideoViews
                .Include(vv => vv.VideoHistory)
                .ToListAsync();
        }

        public async Task<VideoView?> GetVideoViewByIdAsync(int videoViewId)
        {
            if (videoViewId <= 0)
                throw new ArgumentException("VideoView ID must be greater than zero.");

            return await _context.VideoViews
                .Include(vv => vv.VideoHistory)
                .FirstOrDefaultAsync(vv => vv.ViewID == videoViewId);
        }

        public async Task<bool> AddVideoViewAsync(VideoView videoView)
        {
            if (videoView == null)
                throw new ArgumentNullException(nameof(videoView));

            videoView.Quantity = 1;

            await _context.VideoViews.AddAsync(videoView);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateVideoViewAsync(VideoView videoView)
        {
            if (videoView == null || videoView.ViewID <= 0)
                throw new ArgumentException("Invalid VideoView data.");

            var existingVideoView = await GetVideoViewByIdAsync(videoView.ViewID);
            if (existingVideoView == null)
                return false;

            _context.Entry(existingVideoView).CurrentValues.SetValues(videoView);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVideoViewAsync(int videoViewId)
        {
            if (videoViewId <= 0)
                throw new ArgumentException("VideoView ID must be greater than zero.");

            var videoView = await GetVideoViewByIdAsync(videoViewId);
            if (videoView == null)
                return false;

            videoView.Quantity = 0; // Instead of deleting, set Quantity to 0
            _context.VideoViews.Update(videoView);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalViewsForVideoAsync(int videoHistoryId)
        {
            if (videoHistoryId <= 0)
                throw new ArgumentException("VideoHistory ID must be greater than zero.");

            return await _context.VideoViews
                .Where(v => v.VideoHistoryID == videoHistoryId && v.Quantity > 0)
                .SumAsync(v => v.Quantity);
        }

        public async Task<int> CountTotalViewsAsync()
        {
            return await _context.VideoViews.SumAsync(v => v.Quantity);
        }

        public async Task<Dictionary<int, int>> GetViewsCountPerVideoAsync()
        {
            return await _context.VideoViews
                .GroupBy(v => v.VideoHistoryID)
                .Select(g => new { VideoId = g.Key, TotalViews = g.Sum(v => v.Quantity) })
                .ToDictionaryAsync(g => g.VideoId, g => g.TotalViews);
        }
    }
}
