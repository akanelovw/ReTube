namespace ReTube.Dtos.FavoriteVideo
{
    public class FavoriteVideoDetailsDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? VideoFile { get; set; }
    }
}
