using System.ComponentModel.DataAnnotations;

namespace ReTube.Models
{
    public class Video
    {
        public int Id { get; set; }
        [Required]
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? VideoFile { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<FavoriteVideo>? FavoriteVideos { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
    }
}
