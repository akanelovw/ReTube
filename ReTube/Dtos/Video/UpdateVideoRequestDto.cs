namespace ReTube.Dtos.Video
{
    public class UpdateVideoRequestDto
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public string? VideoFile { get; set; }
    }
}
