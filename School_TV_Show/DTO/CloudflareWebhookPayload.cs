namespace School_TV_Show.DTO
{
    public class CloudflareWebhookPayload
    {
        public string Event { get; set; }
        public CloudflareVideoPayload Video { get; set; }
    }

    public class CloudflareVideoPayload
    {
        public CloudflarePlayback Playback { get; set; }
        public string DownloadableUrl { get; set; }
        public CloudflareInput Input { get; set; }
        public string Uid { get; set; }
    }

    public class CloudflareInput
    {
        public string Uid { get; set; }
    }

    public class CloudflarePlayback
    {
        public string Hls { get; set; }
    }
}
