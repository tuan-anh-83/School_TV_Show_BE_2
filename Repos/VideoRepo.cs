﻿using BOs.Models;
using DAOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class VideoRepo : IVideoRepo
    {
        public async Task<List<VideoHistory>> GetAllVideosAsync()
        {
            return await VideoDAO.Instance.GetAllVideosAsync();
        }

        public async Task<VideoHistory?> GetVideoByIdAsync(int videoHistoryId)
        {
            return await VideoDAO.Instance.GetVideoByIdAsync(videoHistoryId);
        }

        public async Task<bool> AddVideoAsync(VideoHistory videoHistory)
        {
            return await VideoDAO.Instance.AddVideoAsync(videoHistory);
        }

        public async Task<bool> UpdateVideoAsync(VideoHistory videoHistory)
        {
            return await VideoDAO.Instance.UpdateVideoAsync(videoHistory);
        }

        public async Task<bool> DeleteVideoAsync(int videoHistoryId)
        {
            return await VideoDAO.Instance.DeleteVideoAsync(videoHistoryId);
        }

        public async Task<IEnumerable<VideoHistory>> GetAllAsync()
        {
            return await VideoDAO.Instance.GetAllVideosAsync();
        }

        public async Task<int> CountByStatusAsync(bool status)
        {
            return await VideoDAO.Instance.CountByStatusAsync(status);
        }

        public async Task<(int totalViews, int totalLikes)> GetTotalViewsAndLikesAsync()
        {
            return await VideoDAO.Instance.GetTotalViewsAndLikesAsync();
        }

        public async Task<int> CountByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await VideoDAO.Instance.CountByDateRangeAsync(startDate, endDate);
        }

        public async Task<int> CountTotalVideosAsync()
        {
            return await VideoDAO.Instance.CountTotalVideosAsync();
        }
        
        public async Task<VideoHistory?> GetLatestLiveStreamByProgramIdAsync(int programId)
        {
            return await VideoDAO.Instance.GetLatestLiveStreamByProgramIdAsync(programId);
        }
    }
}
