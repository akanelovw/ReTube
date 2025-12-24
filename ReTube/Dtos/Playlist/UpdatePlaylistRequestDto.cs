namespace ReTube.Dtos.Playlist
{
    public class UpdatePlaylistRequestDto
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
    }
}
