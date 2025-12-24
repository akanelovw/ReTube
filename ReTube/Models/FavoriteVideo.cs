using System.ComponentModel.DataAnnotations;

namespace ReTube.Models
{
    public class FavoriteVideo
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Video> Videos { get; set; }
    }
}
