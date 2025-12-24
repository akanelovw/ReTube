using ReTube.Models;
using ReTube.Dtos.Video;

namespace ReTube.Mappers
{
    public static class VideoMapper
    {
        public static VideoDto ToVideoDto(this Video videoModel)
        {
            return new VideoDto
            {
                Id = videoModel.Id,
                Title = videoModel.Title,
                Description = videoModel.Description,
                VideoFile = videoModel.VideoFile,
                Image = videoModel.Image,
                User = videoModel.User.UserName
            };
        }

        public static Video ToVideoFromCreateDto(this CreateVideoRequestDto videoDto)
        {
            return new Video
            {
                Title = videoDto.Title,
                Description = videoDto.Description,
                VideoFile = videoDto.VideoFile
            };
        }

    }
}