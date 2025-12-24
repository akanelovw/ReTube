using ReTube.Dtos.FavoriteVideo;

namespace ReTube.Dtos.Playlist
{
    public class PlaylistDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string User { get; set; } = string.Empty;
        public ICollection<PlaylistVideoDto> Videos { get; set; }
    }
}
