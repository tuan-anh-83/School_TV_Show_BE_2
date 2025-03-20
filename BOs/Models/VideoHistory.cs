using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.Models
{
    public class VideoHistory
    {
        public int VideoHistoryID { get; set; }
        public int ProgramID { get; set; }
        public string URL { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public Boolean Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime StreamAt { get; set; }
        public Program Program { get; set; }
        public ICollection<VideoView> VideoViews { get; set; } // Navigation property for VideoView
        public ICollection<VideoLike> VideoLikes { get; set; } // Navigation property
    }
}
