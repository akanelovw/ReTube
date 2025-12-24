namespace ReTube.Dtos.Video
{
    public class VideoDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? VideoFile { get; set; }
        public string User { get; set; } = string.Empty;
    }
}
