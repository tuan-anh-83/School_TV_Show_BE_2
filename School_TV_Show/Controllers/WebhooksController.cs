using Microsoft.AspNetCore.Mvc;
using School_TV_Show.DTO;
using Services;

namespace School_TV_Show.Controllers
{
    [ApiController]
    [Route("api/webhooks")]
    public class WebhooksController : ControllerBase
    {
        private readonly ILiveStreamService _liveStreamService;
        private readonly ILogger<WebhooksController> _logger;

        public WebhooksController(ILiveStreamService liveStreamService, ILogger<WebhooksController> logger)
        {
            _liveStreamService = liveStreamService;
            _logger = logger;
        }

        [HttpPost("cloudflare")]
        public async Task<IActionResult> CloudflareWebhook([FromBody] CloudflareWebhookPayload payload)
        {
            _logger.LogInformation("Received Cloudflare webhook: Event = {Event}", payload.Event);

            if (payload.Video == null)
            {
                _logger.LogWarning(" Payload missing 'video' object.");
                return BadRequest(new { message = "Missing video data in payload." });
            }

            string inputUid = payload.Video.Input?.Uid;
            string videoUid = payload.Video.Uid;
            string hlsUrl = payload.Video.Playback?.Hls;
            string mp4Url = payload.Video.DownloadableUrl;

            _logger.LogInformation(" Webhook Data:\n  VideoUid: {VideoUid}\n  InputUid: {InputUid}\n  HLS: {HlsUrl}\n  MP4: {Mp4Url}",
                videoUid, inputUid, hlsUrl, mp4Url);

            if (payload.Event == "video.ready")
            {
                if (string.IsNullOrEmpty(inputUid))
                {
                    _logger.LogWarning("'input.uid' is missing in payload.");
                    return BadRequest(new { message = "Missing input.uid in video payload." });
                }

                var result = await _liveStreamService.SaveRecordedVideoFromWebhookAsync(inputUid, mp4Url, hlsUrl);

                if (result)
                {
                    _logger.LogInformation("Video saved to DB for inputUid: {InputUid}", inputUid);
                    return Ok(new { message = "Video saved successfully." });
                }
                else
                {
                    _logger.LogWarning("No matching stream found in DB for inputUid: {InputUid}", inputUid);
                    return NotFound(new { message = "No matching stream found." });
                }
            }

            _logger.LogInformation(" Webhook event '{Event}' is not handled.", payload.Event);
            return Ok(new { message = "Event ignored." });
        }
    }
}
