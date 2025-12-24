using ReTube.Models;
using ReTube.Dtos.FavoriteVideo;
using ReTube.Dtos.Video;

namespace ReTube.Mappers
{
    public static class FavoriteVideoMapper
    {
        public static FavoriteVideoDto ToFavoriteVideoDto(this FavoriteVideo favoriteVideoModel)
        {
            return new FavoriteVideoDto
            {
                Id = favoriteVideoModel.Id,
                User = favoriteVideoModel.User.UserName,
                Videos = favoriteVideoModel.Videos.Select(v => v.ToFavoriteVideoVideoDto()).ToList()
            };
        }
        public static FavoriteVideoDetailsDto ToFavoriteVideoVideoDto(this Video videoModel)
        {
            return new FavoriteVideoDetailsDto
            {
                Id = videoModel.Id,
                Title = videoModel.Title,
                Description = videoModel.Description,
                VideoFile = videoModel.VideoFile,
                Image = videoModel.Image
            };
        }
        public static FavoriteVideoToVideo ToFavoriteVideoFromCreateDto(this CreateFavoriteVideoRequestDto favoriteVideoToVideoDto)
        {
            return new FavoriteVideoToVideo
            {
                VideoId = favoriteVideoToVideoDto.VideoId
            };
        }

    }
}