﻿namespace BOs.Models
{
    public class CloudflareSettings
    {
        public string AccountId { get; set; } = string.Empty;
        public string ApiToken { get; set; } = string.Empty;
        public string StreamDomain { get; set; }
        public double Duration { get; set; }
    }
}
