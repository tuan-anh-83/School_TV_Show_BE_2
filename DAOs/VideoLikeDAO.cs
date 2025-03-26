using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAOs
{
    public class VideoLikeDAO
    {
        private static VideoLikeDAO instance = null;
        private readonly DataContext _context;

        private VideoLikeDAO()
        {
            _context = new DataContext();
        }

        public static VideoLikeDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new VideoLikeDAO();
                }
                return instance;
            }
        }

        public async Task<List<VideoLike>> GetAllVideoLikesAsync()
        {
            return await _context.VideoLikes
                .Include(vl => vl.VideoHistory)
                .Include(vl => vl.Account)
                .ToListAsync();
        }

        public async Task<VideoLike?> GetVideoLikeByIdAsync(int videoLikeId)
        {
            if (videoLikeId <= 0)
                throw new ArgumentException("VideoLike ID must be greater than zero.");

            return await _context.VideoLikes
                .Include(vl => vl.VideoHistory)
                .Include(vl => vl.Account)
                .FirstOrDefaultAsync(vl => vl.LikeID == videoLikeId);
        }

        public async Task<bool> AddVideoLikeAsync(VideoLike videoLike)
        {
            if (videoLike == null)
                throw new ArgumentNullException(nameof(videoLike));

            videoLike.Quantity = 1;

            await _context.VideoLikes.AddAsync(videoLike);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateVideoLikeAsync(VideoLike videoLike)
        {
            if (videoLike == null || videoLike.LikeID <= 0)
                throw new ArgumentException("Invalid VideoLike data.");

            var existingVideoLike = await GetVideoLikeByIdAsync(videoLike.LikeID);
            if (existingVideoLike == null)
                return false;

            _context.Entry(existingVideoLike).CurrentValues.SetValues(videoLike);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVideoLikeAsync(int videoLikeId)
        {
            if (videoLikeId <= 0)
                throw new ArgumentException("VideoLike ID must be greater than zero.");

            var videoLike = await GetVideoLikeByIdAsync(videoLikeId);
            if (videoLike == null)
                return false;

            videoLike.Quantity = 0;
            _context.VideoLikes.Update(videoLike);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalLikesForVideoAsync(int videoHistoryId)
        {
            if (videoHistoryId <= 0)
                throw new ArgumentException("VideoHistory ID must be greater than zero.");

            return await _context.VideoLikes
                .Where(s => s.VideoHistoryID == videoHistoryId && s.Quantity > 0)
                .SumAsync(s => s.Quantity);
        }

        public async Task<int> CountTotalLikesAsync()
        {
            return await _context.VideoLikes.CountAsync();
        }

        public async Task<int> CountLikesByVideoIdAsync(int videoHistoryId)
        {
            if (videoHistoryId <= 0)
                throw new ArgumentException("VideoHistory ID must be greater than zero.");

            return await _context.VideoLikes.CountAsync(vl => vl.VideoHistoryID == videoHistoryId);
        }

        public async Task<Dictionary<int, int>> GetLikesCountPerVideoAsync()
        {
            return await _context.VideoLikes
                .GroupBy(vl => vl.VideoHistoryID)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }
    }
}
