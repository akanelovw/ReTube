using ReTube.Dtos.FavoriteVideo;
using ReTube.Dtos.Playlist;
using ReTube.Dtos.Video;
using ReTube.Models;

namespace ReTube.Mappers
{
    public static class PlaylistMapper
    {
        public static PlaylistDto ToPlaylistDto(this Playlist playlistModel)
        {
            return new PlaylistDto
            {
                Id = playlistModel.Id,
                Title = playlistModel.Title,
                Description = playlistModel.Description,
                Image = playlistModel.Image,
                User = playlistModel.User.UserName,
                Videos = playlistModel.Videos.Select(v => v.ToPlaylistVideoDto()).ToList()
            };
        }

        public static UpdatePlaylistRequestDto ToPlaylistCreateDto(this Playlist playlistModel)
        {
            return new UpdatePlaylistRequestDto
            {
                Title = playlistModel.Title,
                Description = playlistModel.Description,
                Image = playlistModel.Image
            };
        }

        public static PlaylistVideoDto ToPlaylistVideoDto(this Video videoModel)
        {
            return new PlaylistVideoDto
            {
                Id = videoModel.Id,
                Title = videoModel.Title,
                Description = videoModel.Description,
                VideoFile = videoModel.VideoFile,
                Image = videoModel.Image
            };
        }

        public static Playlist ToPlaylistFromCreateDto(this CreatePlaylistRequestDto playlistDto)
        {
            return new Playlist
            {
                Title = playlistDto.Title,
                Description = playlistDto.Description,
            };
        }
    }
}
