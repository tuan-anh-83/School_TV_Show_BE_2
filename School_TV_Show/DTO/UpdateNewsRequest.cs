namespace School_TV_Show.DTO
{
    public class UpdateNewsRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool? FollowerMode { get; set; }
        public List<IFormFile> ImageFiles { get; set; }
    }
}
