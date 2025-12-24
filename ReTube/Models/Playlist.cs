using System.ComponentModel.DataAnnotations;

namespace ReTube.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        [Required]
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Video> Videos { get; set; }
    }
}
