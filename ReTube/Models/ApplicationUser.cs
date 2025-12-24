using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace ReTube.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.FavoriteVideos = new FavoriteVideo();
        }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Image { get; set; }
        public List<Video>? Videos { get; set; }
        public FavoriteVideo? FavoriteVideos { get; set; }
        public List<Playlist>? Playlists { get; set; }
    }
}
