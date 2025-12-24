using ReTube.Dtos.Video;

namespace ReTube.Dtos.FavoriteVideo
{
    public class FavoriteVideoDto
    {
        public int Id { get; set; }
        public string User { get; set; } = string.Empty;
        public ICollection<FavoriteVideoDetailsDto> Videos { get; set; }
    }
}
